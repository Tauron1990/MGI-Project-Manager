import { InPlaceEditor } from "@syncfusion/ej2-inplace-editor";
import { HubConnectionBuilder, HubConnection } from "@aspnet/signalr";
import { ApiModule } from "../ApiClient/Client.g";
//import $ from "jquery";

export module FileLinking {

    var linker: FileLinker;

    class FileToName {
        private compled: (inst: FileToName) => void;
        private filesClient: ApiModule.FilesClient;
        private hub: HubConnection;
        private file: ApiModule.UnAssociateFile;
        private template: ApiModule.FileToNameTemplate;

        constructor(template: ApiModule.FileToNameTemplate, file: ApiModule.UnAssociateFile, hub: HubConnection, filesClient: ApiModule.FilesClient,
                    compled: (inst: FileToName) => void) {
            this.compled = compled;
            this.filesClient = filesClient;
            this.hub = hub;
            this.file = file;
            this.template = template;

            this.init();
        }

        private init() {
            
        }

        public hide() {

        }
    }

    class FileLinker {

        hub: HubConnection;
        filesToName: Set<FileToName>;
        filesClient: ApiModule.FilesClient;

        constructor() {
            $("linkingErrorSpan").hide("fast");
        }

        start(id: string) {
            this.filesClient = new ApiModule.FilesClient();
            this.hub = new HubConnectionBuilder()
                .withUrl("/Hubs/Files")
                .build();

            if (id || id === '') {
                this.hub.on("SendMultifileProcessingCompled",
                    arr => {
                        if (id === <string>arr[0]) {
                            this.hub.off("SendMultifileProcessingCompled");
                            if (<boolean>arr[1]) {
                                this.printError(arr[2]);
                            } else {
                                this.requestOperations(id);
                            }
                        }
                    });
            } else {
                this.requestOperations("");
            }

            this.hub.start();
            if (id && id !== '' && id == null){
                this.filesClient.startMultifile(id)
                    .catch(ex => {
                        this.printError(ex.message);
                    });
            }
        }

        private printError(error: string) {
            this.hideProgress();
            const span = $("linkingErrorSpanContent");
            span.html(document.createTextNode(error));
            span.show("slow");
        }

        private requestOperations(id: string) {
            var client = new ApiModule.FilesClient();
            client.getUnAssociateFiles(id)
                .then(this.listFiles)
                .catch(e => {
                    if (e.message) {
                        this.printError(e.message.toString());
                    }
                });
        }

        private hideProgress() {
            $("loadingCircle").hide("slow");
        }

        private listFiles(files: ApiModule.UnAssociateFile[]) {
            if (files.length === 0) {
                this.printFinish();
                return;
            }

            var templateClient = new ApiModule.TemplateClient();
            
            for (let file of files) {
                if (this.filesToName) {
                    this.processFile(file, templateClient);
                } else {
                    this.filesToName = new Set<FileToName>();
                }
            }
        }

        private processFile(file: ApiModule.UnAssociateFile, client: ApiModule.TemplateClient) {
            client.linkingFileTemplate(file)
                .then(ft => {
                    this.filesToName.add(new FileToName(ft, file, this.hub, this.filesClient, this.finish));
                });
        }

        private finish(file: FileToName) {
            file.hide();
            this.filesToName.delete(file);
            if (this.filesToName.size === 0) {
                this.printFinish();
            }
        }

        private printFinish() {
            this.hub.stop();
        }
    }

    export function initOperation(id: string) {
        linker = new FileLinker();
        linker.start(id);
    }
}
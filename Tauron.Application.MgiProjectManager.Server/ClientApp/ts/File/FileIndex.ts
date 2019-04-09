import "jquery";
import "Bootstrap";
import { ApiModule } from "../ApiClient/Client.g";

export module FileIndex {

    declare var resources: [string, string];
    declare function openDialog();

    export var manager: UploadManager;

    class UploadManager {

        message: string;
        operationId: string;

        constructor() {
            $("#progress").hide();
            $("#uploadCompledBox").hide();

            this.wireEvents();
        }

        private wireEvents() {
            const dropSide = $("#fileBasket");

            dropSide.on("dragenter",
                (evt) => {
                    evt.preventDefault();
                    evt.stopPropagation();
                });

            dropSide.on("dragover",
                evt => {
                    evt.preventDefault();
                    evt.stopPropagation();
                });

            dropSide.on("drop", this.dropEvent);
        }

        private dropEvent(evt: JQueryEventObject) {
            evt.preventDefault();
            evt.stopPropagation();

            const orgEvent = evt.originalEvent as DragEvent;
            const files = orgEvent.dataTransfer.files;
            var fileNames = "";

            if (files.length > 0) {
                fileNames = resources["filesIndexUploadLabel"];
                for (let i = 0; i < files.length; i++) {
                    fileNames += files[i].name + "<br />";
                }
            }
            $("#fileBasket").html(fileNames);

            const client = new ApiModule.FilesClient();
            $("#progress").show();

            var data = [];
            for (let i = 0; i < files.length; i++) {
                data.push(files[i]);
            }

            client.files(data)
                .then(ur => {
                    this.disableUpload();
                    this.processUploadResult(ur);
                })
                .catch(e => {
                    var text;
                    if (Array.isArray(e)) {
                        text = e[0].toString();
                    } else {
                        text = e.toString();
                    }

                    $("#fileBasket").html(text);
                    this.showDialog(text, true);
                });
        }

        private disableUpload() {
            $("#fileBasket").css("background", "transparent");
            this.unWireEvents();
        }

        private unWireEvents() {
            const dropSide = $("#fileBasket");

            dropSide.off("dragenter");

            dropSide.off("dragover");

            dropSide.off("drop");
        }

        private processUploadResult(result: ApiModule.UploadResult) {
            var promise: Promise<string>;

            if (result.errors !== null) {
                const error: { [key: string]: string } = result.errors;
                if (Object.keys(error).length > 0) {
                    const client = new ApiModule.TemplateClient();
                    promise = client.fileUploadError(result);
                } else {
                    promise = new Promise(call => call(null));
                }
            } else {
                promise = new Promise(call => call(null));
            }

            promise
                .then(s => this.thenProcess(s, result))
                .catch(e => this.thenProcess(e.toString(), result));
        }

        private thenProcess(ul: string | null, result: ApiModule.UploadResult) {
            this.message = result.message;
            this.operationId = result.operation;

            if (ul !== null) {
                this.showDialog(ul, false);
            } else {
                this.triggerNextPage();
            }
        }

        private showDialog(content: any, reload: boolean) {
            const ele = $("#UploadErrorsContent");
            ele.html(content);

            ele.on("hidden.bs.modal",
                () => {
                    if (reload) {
                        location.reload();
                    } else {
                        this.triggerNextPage();
                    }
                });

            openDialog();
        }

        private triggerNextPage() {

        }
    }

    export function uploadInit() {
        manager = new UploadManager();
    }
}
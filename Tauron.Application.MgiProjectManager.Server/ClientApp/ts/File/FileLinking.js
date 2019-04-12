"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const signalr_1 = require("@aspnet/signalr");
const Client_g_1 = require("../ApiClient/Client.g");
//import $ from "jquery";
var FileLinking;
(function (FileLinking) {
    var linker;
    class FileToName {
        constructor(template, file, hub, filesClient, compled) {
            this.compled = compled;
            this.filesClient = filesClient;
            this.hub = hub;
            this.file = file;
            this.template = template;
            this.init();
        }
        init() {
        }
        hide() {
        }
    }
    class FileLinker {
        constructor() {
            $("linkingErrorSpan").hide("fast");
        }
        start(id) {
            this.filesClient = new Client_g_1.ApiModule.FilesClient();
            this.hub = new signalr_1.HubConnectionBuilder()
                .withUrl("/Hubs/Files")
                .build();
            if (id || id === '') {
                this.hub.on("SendMultifileProcessingCompled", arr => {
                    if (id === arr[0]) {
                        this.hub.off("SendMultifileProcessingCompled");
                        if (arr[1]) {
                            this.printError(arr[2]);
                        }
                        else {
                            this.requestOperations(id);
                        }
                    }
                });
            }
            else {
                this.requestOperations("");
            }
            this.hub.start();
            if (id && id !== '' && id == null) {
                this.filesClient.startMultifile(id)
                    .catch(ex => {
                    this.printError(ex.message);
                });
            }
        }
        printError(error) {
            this.hideProgress();
            const span = $("linkingErrorSpanContent");
            span.html(document.createTextNode(error));
            span.show("slow");
        }
        requestOperations(id) {
            var client = new Client_g_1.ApiModule.FilesClient();
            client.getUnAssociateFiles(id)
                .then(this.listFiles)
                .catch(e => {
                if (e.message) {
                    this.printError(e.message.toString());
                }
            });
        }
        hideProgress() {
            $("loadingCircle").hide("slow");
        }
        listFiles(files) {
            if (files.length === 0) {
                this.printFinish();
                return;
            }
            var templateClient = new Client_g_1.ApiModule.TemplateClient();
            for (let file of files) {
                if (this.filesToName) {
                    this.processFile(file, templateClient);
                }
                else {
                    this.filesToName = new Set();
                }
            }
        }
        processFile(file, client) {
            client.linkingFileTemplate(file)
                .then(ft => {
                this.filesToName.add(new FileToName(ft, file, this.hub, this.filesClient, this.finish));
            });
        }
        finish(file) {
            file.hide();
            this.filesToName.delete(file);
            if (this.filesToName.size === 0) {
                this.printFinish();
            }
        }
        printFinish() {
            this.hub.stop();
        }
    }
    function initOperation(id) {
        linker = new FileLinker();
        linker.start(id);
    }
    FileLinking.initOperation = initOperation;
})(FileLinking = exports.FileLinking || (exports.FileLinking = {}));
//# sourceMappingURL=FileLinking.js.map
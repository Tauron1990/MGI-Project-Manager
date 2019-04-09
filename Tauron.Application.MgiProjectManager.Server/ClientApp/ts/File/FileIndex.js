"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
require("jquery");
require("Bootstrap");
const Client_g_1 = require("../ApiClient/Client.g");
var FileIndex;
(function (FileIndex) {
    class UploadManager {
        constructor() {
            $("#progress").hide();
            $("#uploadCompledBox").hide();
            this.wireEvents();
        }
        wireEvents() {
            const dropSide = $("#fileBasket");
            dropSide.on("dragenter", (evt) => {
                evt.preventDefault();
                evt.stopPropagation();
            });
            dropSide.on("dragover", evt => {
                evt.preventDefault();
                evt.stopPropagation();
            });
            dropSide.on("drop", this.dropEvent);
        }
        dropEvent(evt) {
            evt.preventDefault();
            evt.stopPropagation();
            const orgEvent = evt.originalEvent;
            const files = orgEvent.dataTransfer.files;
            var fileNames = "";
            if (files.length > 0) {
                fileNames = resources["filesIndexUploadLabel"];
                for (let i = 0; i < files.length; i++) {
                    fileNames += files[i].name + "<br />";
                }
            }
            $("#fileBasket").html(fileNames);
            const client = new Client_g_1.ApiModule.FilesClient();
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
                }
                else {
                    text = e.toString();
                }
                $("#fileBasket").html(text);
                this.showDialog(text, true);
            });
        }
        disableUpload() {
            $("#fileBasket").css("background", "transparent");
            this.unWireEvents();
        }
        unWireEvents() {
            const dropSide = $("#fileBasket");
            dropSide.off("dragenter");
            dropSide.off("dragover");
            dropSide.off("drop");
        }
        processUploadResult(result) {
            var promise;
            if (result.errors !== null) {
                const error = result.errors;
                if (Object.keys(error).length > 0) {
                    const client = new Client_g_1.ApiModule.TemplateClient();
                    promise = client.fileUploadError(result);
                }
                else {
                    promise = new Promise(call => call(null));
                }
            }
            else {
                promise = new Promise(call => call(null));
            }
            promise
                .then(s => this.thenProcess(s, result))
                .catch(e => this.thenProcess(e.toString(), result));
        }
        thenProcess(ul, result) {
            this.message = result.message;
            this.operationId = result.operation;
            if (ul !== null) {
                this.showDialog(ul, false);
            }
            else {
                this.triggerNextPage();
            }
        }
        showDialog(content, reload) {
            const ele = $("#UploadErrorsContent");
            ele.html(content);
            ele.on("hidden.bs.modal", () => {
                if (reload) {
                    location.reload();
                }
                else {
                    this.triggerNextPage();
                }
            });
            openDialog();
        }
        triggerNextPage() {
        }
    }
    function uploadInit() {
        FileIndex.manager = new UploadManager();
    }
    FileIndex.uploadInit = uploadInit;
})(FileIndex = exports.FileIndex || (exports.FileIndex = {}));
//# sourceMappingURL=FileIndex.js.map
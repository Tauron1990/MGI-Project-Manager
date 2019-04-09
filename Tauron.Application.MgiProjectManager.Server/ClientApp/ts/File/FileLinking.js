"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const signalr_1 = require("@aspnet/signalr");
//import $ from "jquery";
var FileLinking;
(function (FileLinking) {
    var linker;
    class FileLinker {
        constructor() {
            $("linkingErrorSpan").hide("fast");
        }
        start(id) {
            var hub = new signalr_1.HubConnectionBuilder()
                .withUrl("/Hubs/Files")
                .build();
            if (id || id === '') {
                hub.on("SendMultifileProcessingCompled", arr => {
                    if (id === arr[0]) {
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
            hub.start();
        }
        printError(error) {
            this.hideProgress();
            var span = $("linkingErrorSpanContent");
            span.html(document.createTextNode(error));
            span.show("slow");
        }
        requestOperations(id) {
        }
        hideProgress() {
            $("circle").hide("slow");
        }
    }
    function initOperation(id) {
        linker = new FileLinker();
        linker.start(id);
    }
    FileLinking.initOperation = initOperation;
})(FileLinking = exports.FileLinking || (exports.FileLinking = {}));
//# sourceMappingURL=FileLinking.js.map
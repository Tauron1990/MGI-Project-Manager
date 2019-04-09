import { HubConnectionBuilder } from "@aspnet/signalr"
//import $ from "jquery";

export module FileLinking {

    var linker: FileLinker;

    class FileLinker {

        constructor() {
            $("linkingErrorSpan").hide("fast");
        }

        public start(id: string) {
            var hub = new HubConnectionBuilder()
                .withUrl("/Hubs/Files")
                .build();

            if (id || id === '') {
                hub.on("SendMultifileProcessingCompled", arr => {
                    if (id === <string>arr[0]) {
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

            hub.start();
        }

        printError(error: string) {
            this.hideProgress();
            var span = $("linkingErrorSpanContent");
            span.html(document.createTextNode(error));
            span.show("slow");
        }

        requestOperations(id: string) {

        }

        hideProgress() {
            $("circle").hide("slow");
        }
    }

    export function initOperation(id: string) {
        linker = new FileLinker();
        linker.start(id);
    }
}
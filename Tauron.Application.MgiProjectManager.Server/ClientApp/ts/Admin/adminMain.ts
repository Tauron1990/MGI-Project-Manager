//import "@syncfusion/ej2/grids";
import "jquery";
import { Dialog } from "@syncfusion/ej2/popups";
import { ApiModule } from "../ApiClient/Client.g";


export module Admin {

    var manager: GridManager;

    export class GridManager {
        public dialog: Dialog;

        actionFailure(args: any): void {
            let temp;
            if (args.hasOwnProperty("error")) {
                temp = args.error[0].error;
            } else {
                temp = args[0].error;
            }

            this.formatError(temp);
        }

        private formatError(xhr: XMLHttpRequest) {
            const text = xhr.responseText;

            const contentspan = $("#ErrorListSpan");

            if (text.charAt(0) === "{") {

                const client = new ApiModule.TemplateClient();
                client.adminGridError(text)
                    .then(s => contentspan.html(s))
                    .catch(e => {
                        if (Array.isArray(e)) {
                            contentspan.html(e[0]);
                        } else {
                            contentspan.html(e);
                        }
                    });

                //const json = JSON.parse(text);
                //const errors = json.errors;
                //const ul = document.createElement("ul");

                //for (let propertyName in errors) {
                //    if (errors.hasOwnProperty(propertyName)) {

                //        const il = document.createElement("li");
                //        il.className = "text-danger";

                //        il.appendChild(document.createTextNode(errors[propertyName]));
                //        ul.appendChild(il);
                //    }
                //}
            } else {
                contentspan.html(text);
            }

            this.dialog.show();
        }
    }

    export function init() {
        manager = new GridManager();
    }

    export function actionFailure(args: any) {  manager.actionFailure(args); }
}
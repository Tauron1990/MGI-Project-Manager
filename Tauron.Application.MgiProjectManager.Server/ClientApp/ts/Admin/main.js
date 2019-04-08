"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
//import "@syncfusion/ej2/grids";
require("jquery");
const Client_g_1 = require("../ApiClient/Client.g");
var Admin;
(function (Admin) {
    var manager;
    class GridManager {
        actionFailure(args) {
            let temp;
            if (args.hasOwnProperty("error")) {
                temp = args.error[0].error;
            }
            else {
                temp = args[0].error;
            }
            this.formatError(temp);
        }
        formatError(xhr) {
            const text = xhr.responseText;
            const contentspan = $("#ErrorListSpan");
            if (text.charAt(0) === "{") {
                const client = new Client_g_1.ApiModule.TemplateClient();
                client.adminGridError(text)
                    .then(s => contentspan.html(s))
                    .catch(e => {
                    if (Array.isArray(e)) {
                        contentspan.html(e[0]);
                    }
                    else {
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
            }
            else {
                contentspan.html(text);
            }
            this.dialog.show();
        }
    }
    Admin.GridManager = GridManager;
    function init() {
        manager = new GridManager();
    }
    Admin.init = init;
    function actionFailure(args) { manager.actionFailure(args); }
    Admin.actionFailure = actionFailure;
})(Admin = exports.Admin || (exports.Admin = {}));
//# sourceMappingURL=main.js.map
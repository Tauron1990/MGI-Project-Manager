import "./Lib.js.js";
import { Dialog } from "@syncfusion/ej2-popups";

declare var objects: {};

export function addObject(name: string, obj: any) {
    if (objects === null) {
        objects = {};
    }

    objects[name] = obj;
}

export function actionFailure(args: any) {
    let temp;
    if (args.hasOwnProperty("error")) {
        temp = args.error[0].error;
    } else {
        temp = args[0].error;
    }

    formatError(temp);
}

export function redictToHome() {
    window.setTimeout(() => {
            window.location.replace("/Index");
        },
        3000);
}

// ReSharper disable once InconsistentNaming
export function SetupFormCheck(url = null) {
    var form = document.forms["SetupForm"];

    if (form != null) {
        form.submit();
        return;
    }

    if (url != null) {
        window.location.replace(url);
    }
}

// ReSharper disable once InconsistentNaming
export function ToggleShowPass(passfield) {
    var ele = document.getElementById(passfield) as HTMLInputElement;

    if (ele != null) {
        if (ele.type === "text") {
            ele.type = "password";
        } else {
            ele.type = "text";
        }
    }
}

function getObject<T>(name: string) : T {
    return objects[name] as T;
}

function formatError(xhr: XMLHttpRequest) {
    const text = xhr.responseText;

    const contentspan = $("#ErrorListSpan");

    if (text.charAt(0) === "{") {

        const json = JSON.parse(text);
        const errors = json.errors;
        const ul = document.createElement("ul");

        for (let propertyName in errors) {
            if (errors.hasOwnProperty(propertyName)) {

                const il = document.createElement("li");
                il.className = "text-danger";

                il.appendChild(document.createTextNode(errors[propertyName]));
                ul.appendChild(il);
            }
        }
        contentspan.html(ul);
    } else {
        contentspan.html(text);
    }
    
    var dialogObj = getObject<Dialog>("ErrorList");
    dialogObj.show();
}
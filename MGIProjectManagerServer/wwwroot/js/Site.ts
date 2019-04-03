import { Dialog } from "@syncfusion/ej2-popups";

declare var objects: {};
declare var filesIndexUploadLabel: string;

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

export function uploadInit() {
    $("#progress").hide();

    const dropSide = $("#fileBasket");

    dropSide.on("dragenter", (evt) => {
        evt.preventDefault();
        evt.stopPropagation();
    });

    dropSide.on("dragover", evt => {
        evt.preventDefault();
        evt.stopPropagation();
    });

    dropSide.on("drop",
        (evt: JQueryEventObject) => {
            evt.preventDefault();
            evt.stopPropagation();

            const orgEvent = evt.originalEvent as DragEvent;
            const files = orgEvent.dataTransfer.files;
            var fileNames = "";

            if (files.length > 0) {
                fileNames = filesIndexUploadLabel;
                for (let i = 0; i < files.length; i++) {
                    fileNames += files[i].name + "<br />";
                }
            }
            $("#fileBasket").html(fileNames);

            var data = new FormData();
            for (let i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }

            $("#progress").show();

            const request = new XMLHttpRequest();
            request.onreadystatechange = () => {

                if (request.readyState === 4) {
                    $("#progress").hide();
                    $("#fileBasket").html(request.responseText);
                }
            }

            request.onerror = () => {

                $("#fileBasket").html(request.responseText);
            }
            
            request.onprogress = (ev: ProgressEvent) => {
                let percent: number = 100 / ev.total * ev.loaded;
                $('#progressBar').attr('aria-valuenow', percent).css('width', percent + "%");
            }

            request.open("POST", "api/files");
            request.send(data);
        });
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
declare var resources: [string, string];
declare var operationId;
declare var message;



export function uploadInit() {
    $("#progress").hide();
    $("#uploadCompledBox").hide();
    
    wireEvents();
}

function dropEvent(evt: JQueryEventObject) {
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

    var data = new FormData();
    for (let i = 0; i < files.length; i++) {
        data.append(files[i].name, files[i]);
    }

    $("#progress").show();

    const request = new XMLHttpRequest();
    //request.overrideMimeType("multipart/form-data");

    request.onreadystatechange = () => {

        if (request.readyState === 4) {
            $("#progress").hide();
            disableUpload();
            processUploadResult(request.responseText, request.status);
        }
    }

    request.onerror = () => {

        $("#fileBasket").html(request.responseText);
    }

    request.onprogress = (ev: ProgressEvent) => {
        const percent: number = 100 / ev.total * ev.loaded;
        $("#progressBar").attr('aria-valuenow', percent).css('width', percent + "%");
    }

    request.open("POST", "api/files");
    request.send(data);
}

function wireEvents() {
    const dropSide = $("#fileBasket");

    dropSide.on("dragenter", (evt) => {
        evt.preventDefault();
        evt.stopPropagation();
    });

    dropSide.on("dragover", evt => {
        evt.preventDefault();
        evt.stopPropagation();
    });

    dropSide.on("drop", dropEvent);
}

function disableUpload() {
    $("#fileBasket").css("background", "transparent");
    unWireEvents();
}

function unWireEvents() {
    const dropSide = $("#fileBasket");

    dropSide.off("dragenter");

    dropSide.off("dragover");

    dropSide.off("drop");
}

function processUploadResult(jsonText: string, status: number) {
    if (status !== 200) {
        showDialog(jsonText, true);
    }

    const json = JSON.parse(jsonText);

    let ul = undefined;


    if (json.errors) {
        const errors = json.errors;

        for (let propertyName in errors) {
            if (errors.hasOwnProperty(propertyName)) {

                const il = document.createElement("li");
                il.className = "text-danger";

                il.appendChild(document.createTextNode(errors[propertyName]));
                if (ul) {
                    ul.appendChild(il);
                } else {
                    ul = document.createElement("ul");
                    ul.appendChild(il);
                }

            }
        }
    }

    message = json.message;
    operationId = json.operation;

    if (ul) {
        showDialog(ul, false);
    } else {
        triggerNextPage();
    }
}

function showDialog(content: any, reload: boolean) {
    const ele = $("#UploadErrorsContent");
    ele.html(content);

    ele.on("hidden.bs.modal",
        () => {
            if (reload) {
                location.reload();
            } else {
                triggerNextPage();
            }
        });

    $("#UploadErrorsDialog").modal({
        keyboard: true,
        show: true
    });
}

function triggerNextPage() {
    
}
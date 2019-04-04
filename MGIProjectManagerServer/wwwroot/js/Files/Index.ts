declare var filesIndexUploadLabel: string;

export function uploadInit() {
    $("#progress").hide();
    //$("#uploadCompledBox").hide();
    wireEvents();
}

function dropEvent(evt: JQueryEventObject) {
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
            disableUpload();
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
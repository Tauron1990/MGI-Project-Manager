declare var filesIndexUploadLabel: string;

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

    dropSide.on("drop", dropEvent);
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
            $("#fileBasket").off("drop");
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
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function uploadInit() {
    $("#progress").hide();
    //$("#uploadCompledBox").hide();
    wireEvents();
}
exports.uploadInit = uploadInit;
function dropEvent(evt) {
    evt.preventDefault();
    evt.stopPropagation();
    var orgEvent = evt.originalEvent;
    var files = orgEvent.dataTransfer.files;
    var fileNames = "";
    if (files.length > 0) {
        fileNames = filesIndexUploadLabel;
        for (var i = 0; i < files.length; i++) {
            fileNames += files[i].name + "<br />";
        }
    }
    $("#fileBasket").html(fileNames);
    var data = new FormData();
    for (var i = 0; i < files.length; i++) {
        data.append(files[i].name, files[i]);
    }
    $("#progress").show();
    var request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (request.readyState === 4) {
            $("#progress").hide();
            $("#fileBasket").html(request.responseText);
            disableUpload();
        }
    };
    request.onerror = function () {
        $("#fileBasket").html(request.responseText);
    };
    request.onprogress = function (ev) {
        var percent = 100 / ev.total * ev.loaded;
        $('#progressBar').attr('aria-valuenow', percent).css('width', percent + "%");
    };
    request.open("POST", "api/files");
    request.send(data);
}
function wireEvents() {
    var dropSide = $("#fileBasket");
    dropSide.on("dragenter", function (evt) {
        evt.preventDefault();
        evt.stopPropagation();
    });
    dropSide.on("dragover", function (evt) {
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
    var dropSide = $("#fileBasket");
    dropSide.off("dragenter");
    dropSide.off("dragover");
    dropSide.off("drop");
}
//# sourceMappingURL=Index.js.map
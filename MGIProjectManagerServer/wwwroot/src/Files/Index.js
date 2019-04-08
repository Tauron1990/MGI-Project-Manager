"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function uploadInit() {
    $("#progress").hide();
    $("#uploadCompledBox").hide();
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
        fileNames = resources["filesIndexUploadLabel"];
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
    //request.overrideMimeType("multipart/form-data");
    request.onreadystatechange = function () {
        if (request.readyState === 4) {
            $("#progress").hide();
            disableUpload();
            processUploadResult(request.responseText, request.status);
        }
    };
    request.onerror = function () {
        $("#fileBasket").html(request.responseText);
    };
    request.onprogress = function (ev) {
        var percent = 100 / ev.total * ev.loaded;
        $("#progressBar").attr('aria-valuenow', percent).css('width', percent + "%");
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
function processUploadResult(jsonText, status) {
    if (status !== 200) {
        showDialog(jsonText, true);
    }
    var json = JSON.parse(jsonText);
    var ul = undefined;
    if (json.errors) {
        var errors = json.errors;
        for (var propertyName in errors) {
            if (errors.hasOwnProperty(propertyName)) {
                var il = document.createElement("li");
                il.className = "text-danger";
                il.appendChild(document.createTextNode(errors[propertyName]));
                if (ul) {
                    ul.appendChild(il);
                }
                else {
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
    }
    else {
        triggerNextPage();
    }
}
function showDialog(content, reload) {
    var ele = $("#UploadErrorsContent");
    ele.html(content);
    ele.on("hidden.bs.modal", function () {
        if (reload) {
            location.reload();
        }
        else {
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
//# sourceMappingURL=Index.js.map
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function addObject(name, obj) {
    if (objects === null) {
        objects = {};
    }
    objects[name] = obj;
}
exports.addObject = addObject;
function actionFailure(args) {
    var temp;
    if (args.hasOwnProperty("error")) {
        temp = args.error[0].error;
    }
    else {
        temp = args[0].error;
    }
    formatError(temp);
}
exports.actionFailure = actionFailure;
function uploadInit() {
    var _this = this;
    $("#progress").hide();
    var dropSide = $("#fileBasket");
    dropSide.on("dragenter", function (evt) {
        evt.preventDefault();
        evt.stopPropagation();
    });
    dropSide.on("dragover", function (evt) {
        evt.preventDefault();
        evt.stopPropagation();
    });
    dropSide.on("drop", function (evt) {
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
            var reqthis = _this;
            if (reqthis.readyState === 4) {
                $("#progress").hide();
                $("#fileBasket").html(reqthis.responseText);
            }
        };
        request.onerror = function () {
            var reqthis = _this;
            $("#fileBasket").html(reqthis.responseText);
        };
        request.onprogress = function (ev) {
            var percent = 100 / ev.total * ev.loaded;
            $('#progressBar').attr('aria-valuenow', percent).css('width', percent + "%");
        };
        request.open("POST", "/Files/UploadFiles");
        request.send(data);
    });
}
exports.uploadInit = uploadInit;
function redictToHome() {
    window.setTimeout(function () {
        window;
    }, 2000);
}
exports.redictToHome = redictToHome;
function SetupFormCheck(url) {
    if (url === void 0) { url = null; }
    var form = document.forms["SetupForm"];
    if (form != null) {
        form.submit();
        return;
    }
    if (url != null) {
        window.location.replace(url);
    }
}
exports.SetupFormCheck = SetupFormCheck;
function ToggleShowPass(passfield) {
    var ele = document.getElementById(passfield);
    if (ele != null) {
        if (ele.type === "text") {
            ele.type = "password";
        }
        else {
            ele.type = "text";
        }
    }
}
exports.ToggleShowPass = ToggleShowPass;
function getObject(name) {
    return objects[name];
}
function formatError(xhr) {
    var text = xhr.responseText;
    var contentspan = $("#ErrorListSpan");
    if (text.charAt(0) === "{") {
        var json = JSON.parse(text);
        var errors = json.errors;
        var ul = document.createElement("ul");
        for (var propertyName in errors) {
            if (errors.hasOwnProperty(propertyName)) {
                var il = document.createElement("li");
                il.className = "text-danger";
                il.appendChild(document.createTextNode(errors[propertyName]));
                ul.appendChild(il);
            }
        }
        contentspan.html(ul);
    }
    else {
        contentspan.html(text);
    }
    var dialogObj = getObject("ErrorList");
    dialogObj.show();
}
//# sourceMappingURL=Site.js.map
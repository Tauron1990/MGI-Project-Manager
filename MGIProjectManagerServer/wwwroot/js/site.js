// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

function SetupFormCheck(url = null) {
    var form = document.forms["SetupForm"];

    if (form != null) {
        form.submit();
        return;
    }

    if (url != null) {
        window.location.replace(url);
    }
}

function ToggleShowPass(passfield) {
    var ele = document.getElementById(passfield);

    if (ele != null) {
        if (ele.type === "text") {
            ele.type = "password";
        } else {
            ele.type = "text";
        }
    }
}
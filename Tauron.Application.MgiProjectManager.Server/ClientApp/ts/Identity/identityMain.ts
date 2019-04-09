export function redictToHome() {
    window.setTimeout(() => {
            window.location.replace("/Index");
        },
        3000);
}

export function toggleShowPass(passfield) {
    var ele = document.getElementById(passfield) as HTMLInputElement;

    if (ele != null) {
        if (ele.type === "text") {
            ele.type = "password";
        } else {
            ele.type = "text";
        }
    }
}
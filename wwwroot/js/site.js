
document.addEventListener("DOMContentLoaded", function () {

    const registerForm = document.getElementById("registerForm");

    if (!registerForm) {
        return;
    }

    registerForm.addEventListener("submit", function (event) {

        const password = document.getElementById("Password").value;
        const confirmPassword = document.getElementById("ConfirmPassword").value;

        if (password !== confirmPassword) {
            event.preventDefault();

            alert("Password and Confirm Password do not match.");

            document.getElementById("ConfirmPassword").focus();
        }

    });

});

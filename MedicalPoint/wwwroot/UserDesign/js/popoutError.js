function popout({ error }) {

    var pagebutton = document.getElementById("popoutButton");

    var errorMassage = document.getElementById("errorText");
    error == '' ? errorMassage.innerHTML = 'هناك خطأ ما' : errorMassage.innerHTML = error;
    pagebutton.click();
}
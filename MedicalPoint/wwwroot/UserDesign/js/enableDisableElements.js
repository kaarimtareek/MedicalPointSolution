function enableDisable({ ids }) {

    if (ids == null)
        return;

    for (i = 0; i < ids.length; i++) {

        var id = ids[i];
        var element = document.getElementById(id);
        if (element != null) {
            element.style.display = element.style.display == "none" ? "" : "none";
        }
    }

}
function ResetFilters(ids) {
    if (ids == null)
        return;
    for (i = 0; i < ids.length; i++) {
        var id = ids[i];
        var element = document.getElementById(id);
        if (element != null) {
            element.value = null;
            element.selected = null;
            element.checked = null;
        }
    }
}
// change inputs status
const el = document.getElementById('my-select');
disable(['milNum', 'natNum', 'saryaNum', 'generalNum', 'majorInput', 'natNum']);

el.addEventListener('change', function handleChange(event) {
    //this is for student
    //student => military , saraya, major, genral
    if (event.target.options[event.target.selectedIndex].text === 'طالب') {
        enable(['milNum', 'natNum', 'saryaNum', 'generalNum', 'majorInput']);
        disable(['natNum']);
    }
    else if (event.target.options[event.target.selectedIndex].text === 'مدني') {
        //this is for ordinary citizens

        disable(['milNum', 'natNum', 'saryaNum', 'generalNum', 'majorInput']);
        enable(['natNum']);

     
    }
    //this is for default value ( disable all)
    else if (event.target.options[event.target.selectedIndex].text === 'الدرجة\\الرتبة') {

        disable(['milNum', 'natNum', 'saryaNum', 'generalNum', 'majorInput', 'natNum']);
    }
    //for all degrees
    else {
        enable(['milNum']);
        disable(['natNum', 'natNum', 'saryaNum', 'generalNum', 'majorInput']);
      
    }
});
function enable(ids) {
    if (ids == null)
        return;
    for (i = 0; i < ids.length; i++) {
        var id = ids[i];
        e = document.getElementById(id);
        e.disabled = false;
        e.style = "opacity:1";
    }
}
function disable(ids) {
    if (ids == null)
        return;
    for (i = 0; i < ids.length; i++) {
        var id = ids[i];
        e = document.getElementById(id);
        e.disabled = true;
        e.style = "opacity:0.5";
    }
}
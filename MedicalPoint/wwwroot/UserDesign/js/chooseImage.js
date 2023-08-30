document.querySelector("#files").onchange = function() {
    const fileName = this.files[0]?.name;
    const label = document.querySelector("label[for=files]");
    label.innerText =  'لقد إخترت'+' '+fileName  ?? "إختر الصورة";
  };
  document.querySelector("#files-two").onchange = function() {
    const fileName = this.files[0]?.name;
    const label = document.querySelector("label[for=files-two]");
    label.innerText =  'لقد إخترت'+' '+fileName  ?? "إختر الصورة";
  };
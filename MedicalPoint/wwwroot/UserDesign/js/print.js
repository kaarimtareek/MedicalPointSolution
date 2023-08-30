document.getElementById('printConfirm').onclick
    = function (param) {
        if (!confirm('هل تريد طباعة البيانات؟')) {
            param.preventDefault();
        }
    }




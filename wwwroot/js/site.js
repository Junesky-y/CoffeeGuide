// Загальна поведінка сайту.
// Фільтри працюють і без JavaScript (кнопка «Застосувати»); тут лише зручність:
// форми з атрибутом data-autosubmit відправляються одразу після вибору значення.
document.querySelectorAll('form[data-autosubmit] select').forEach(function (select) {
    select.addEventListener('change', function () {
        select.form.submit();
    });
});

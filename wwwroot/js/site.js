// Загальна поведінка сайту.

// Фільтри працюють і без JavaScript (кнопка «Застосувати»); тут лише зручність:
// форми з атрибутом data-autosubmit відправляються одразу після вибору значення.
document.querySelectorAll('form[data-autosubmit] select').forEach(function (select) {
    select.addEventListener('change', function () {
        select.form.submit();
    });
});

// Автофокус лише на комп'ютері: на телефоні він одразу відкриває клавіатуру й закриває половину екрана.
if (window.matchMedia('(min-width: 768px) and (hover: hover)').matches) {
    var field = document.querySelector('[data-autofocus-desktop]');
    if (field) field.focus();
}

// Шапка: якщо назва сайту не вміщається в рядку (вузький екран або довга назва), у шапці лишається
// лише логотип, а назва виводиться зверху в розкритому меню. Для цього додається клас brand-compact.
(function () {
    var nav = document.querySelector('.navbar-coffee');
    var text = nav && nav.querySelector('.brand-text');
    if (!nav || !text) return;

    var mobile = window.matchMedia('(max-width: 1199.98px)');
    var frame = null;

    function update() {
        nav.classList.remove('brand-compact');                // спершу показуємо назву й вимірюємо
        if (mobile.matches && text.scrollWidth > text.clientWidth + 1) {
            nav.classList.add('brand-compact');               // текст обрізається — ховаємо його з шапки
        }
    }

    function schedule() {
        if (frame) cancelAnimationFrame(frame);
        frame = requestAnimationFrame(update);
    }

    window.addEventListener('resize', schedule);
    window.addEventListener('orientationchange', schedule);
    window.addEventListener('load', schedule);
    if (document.fonts && document.fonts.ready) document.fonts.ready.then(schedule);
    update();
})();

// Єдиний компонент модального вікна для зерен, молока, сиропів, способів приготування та рецептів.
// Будь-який елемент з атрибутом data-details-url відкриває вікно: скрипт завантажує JSON з сервера
// (Razor Page Handler "Details") і малює його. Усі тексти встановлюються через textContent,
// тому розмітка з даних ніколи не інтерпретується як HTML.
(function () {
    'use strict';

    var modalEl = document.getElementById('detailsModal');
    if (!modalEl || typeof bootstrap === 'undefined') return;

    var modal = bootstrap.Modal.getOrCreateInstance(modalEl);

    var el = {
        title: document.getElementById('detailsModalTitle'),
        subtitle: document.getElementById('detailsModalSubtitle'),
        state: document.getElementById('detailsModalState'),
        stateText: document.getElementById('detailsModalStateText'),
        spinner: document.getElementById('detailsModalSpinner'),
        retry: document.getElementById('detailsModalRetry'),
        content: document.getElementById('detailsModalContent'),
        image: document.getElementById('detailsModalImage'),
        summary: document.getElementById('detailsModalSummary'),
        sections: document.getElementById('detailsModalSections'),
        body: modalEl.querySelector('.modal-body')
    };

    var messages = {
        loading: modalEl.dataset.msgLoading,
        error: modalEl.dataset.msgError,
        notFound: modalEl.dataset.msgNotfound
    };

    var cache = new Map();      // уже завантажені відповіді: повторне відкриття не робить запит
    var currentUrl = null;
    var controller = null;      // дозволяє скасувати застарілий запит
    var lastTrigger = null;     // елемент, на який повернеться фокус після закриття

    // Один обробник для всіх карток, а також для посилань усередині самого вікна
    document.addEventListener('click', function (event) {
        var trigger = event.target.closest('[data-details-url]');
        if (!trigger) return;

        event.preventDefault();
        if (!modalEl.contains(trigger)) lastTrigger = trigger;
        openDetails(trigger.dataset.detailsUrl);
    });

    el.retry.addEventListener('click', function () {
        if (currentUrl) load(currentUrl);
    });

    modalEl.addEventListener('hidden.bs.modal', function () {
        if (controller) controller.abort();
        currentUrl = null;
        reset();
        if (lastTrigger && document.contains(lastTrigger)) lastTrigger.focus();
    });

    function openDetails(url) {
        currentUrl = url;
        reset();                                   // попередні дані не лишаються на екрані
        if (!modalEl.classList.contains('show')) modal.show();
        load(url);
    }

    function load(url) {
        if (controller) controller.abort();

        if (cache.has(url)) {
            render(cache.get(url));
            return;
        }

        controller = new AbortController();
        showState(messages.loading, true, false);

        fetch(url, { headers: { Accept: 'application/json' }, signal: controller.signal })
            .then(function (response) {
                if (response.status === 404) {
                    if (url === currentUrl) showState(messages.notFound, false, false);
                    return null;
                }
                if (!response.ok) throw new Error('HTTP ' + response.status);
                return response.json();
            })
            .then(function (data) {
                if (!data) return;
                cache.set(url, data);
                if (url === currentUrl) render(data);
            })
            .catch(function (error) {
                if (error && error.name === 'AbortError') return;
                if (url === currentUrl) showState(messages.error, false, true);
            });
    }

    // ----- Стани -----

    function reset() {
        el.title.textContent = '';
        el.subtitle.textContent = '';
        el.subtitle.hidden = true;
        el.summary.textContent = '';
        el.sections.replaceChildren();
        el.image.hidden = true;
        el.image.removeAttribute('src');
        el.image.classList.remove('is-placeholder');
        el.content.hidden = true;
        el.state.hidden = true;
    }

    function showState(text, loading, canRetry) {
        el.content.hidden = true;
        el.state.hidden = false;
        el.stateText.textContent = text;
        el.spinner.hidden = !loading;
        el.retry.hidden = !canRetry;
    }

    // ----- Малювання даних -----

    function render(data) {
        el.state.hidden = true;
        el.content.hidden = false;

        el.title.textContent = data.title || '';
        el.subtitle.textContent = data.subtitle || '';
        el.subtitle.hidden = !data.subtitle;
        el.summary.textContent = data.summary || '';

        // Фото запису або, якщо його немає, зображення-заглушка з сервера
        var hasPhoto = !!data.imageUrl;
        el.image.src = hasPhoto ? data.imageUrl : (data.placeholder || '');
        el.image.alt = hasPhoto ? (data.title || '') : '';
        el.image.classList.toggle('is-placeholder', !hasPhoto);
        el.image.hidden = !el.image.getAttribute('src');

        el.sections.replaceChildren();
        (data.sections || []).forEach(function (section) {
            el.sections.appendChild(renderSection(section));
        });

        el.body.scrollTop = 0;
        el.title.focus({ preventScroll: true });
    }

    function renderSection(section) {
        var wrap = document.createElement('section');
        wrap.className = 'details-section';

        if (section.heading) {
            var heading = document.createElement('h3');
            heading.className = 'details-heading';
            heading.textContent = section.heading;
            wrap.appendChild(heading);
        }

        switch (section.type) {
            case 'text':
                (section.text || '').split('\n').forEach(function (line) {
                    if (!line.trim()) return;
                    var p = document.createElement('p');
                    p.className = 'mb-2';
                    p.textContent = line;
                    wrap.appendChild(p);
                });
                break;

            case 'list':
            case 'ordered':
                var list = document.createElement(section.type === 'ordered' ? 'ol' : 'ul');
                (section.items || []).forEach(function (text) {
                    var li = document.createElement('li');
                    li.textContent = text;
                    list.appendChild(li);
                });
                wrap.appendChild(list);
                break;

            case 'facts':
                var dl = document.createElement('dl');
                dl.className = 'row details-facts mb-0';
                (section.facts || []).forEach(function (fact) {
                    var dt = document.createElement('dt');
                    dt.className = 'col-sm-4';
                    dt.textContent = fact.label;
                    var dd = document.createElement('dd');
                    dd.className = 'col-sm-8';
                    dd.textContent = fact.value;
                    dl.appendChild(dt);
                    dl.appendChild(dd);
                });
                wrap.appendChild(dl);
                break;

            case 'links':
                var box = document.createElement('div');
                box.className = 'd-flex flex-wrap gap-2';
                (section.links || []).forEach(function (link) {
                    var button = document.createElement('button');
                    button.type = 'button';
                    button.className = 'btn btn-sm btn-outline-coffee';
                    button.textContent = link.text;
                    button.dataset.detailsUrl = link.url;   // той самий механізм: відкриває інший запис
                    box.appendChild(button);
                });
                wrap.appendChild(box);
                break;
        }

        return wrap;
    }
})();

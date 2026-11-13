(() => {
    'use strict';

    const config = {
        users: {
            title: 'Користувачі', endpoint: '/api/users',
            columns: [
                ['id', 'ID'], ['username', 'Логін'], ['email', 'Електронна пошта'],
                ['isAdmin', 'Роль'], ['isRegistrationConfirmed', 'Статус'], ['registrationDate', 'Дата реєстрації']
            ],
            fields: [
                { name: 'username', label: 'Логін', required: true },
                { name: 'email', label: 'Електронна пошта', type: 'email', required: true },
                { name: 'isAdmin', label: 'Права адміністратора', type: 'checkbox' },
                { name: 'isRegistrationConfirmed', label: 'Реєстрацію підтверджено', type: 'checkbox' }
            ],
            createFields: [
                { name: 'username', label: 'Логін', required: true },
                { name: 'email', label: 'Електронна пошта', type: 'email', required: true },
                { name: 'password', label: 'Пароль', type: 'password', required: true, minLength: 8 }
            ]
        },
        artists: {
            title: 'Виконавці', endpoint: '/api/artists',
            columns: [['id', 'ID'], ['name', 'Ім’я'], ['country', 'Країна'], ['recordCount', 'Композицій']],
            fields: [
                { name: 'name', label: 'Ім’я виконавця', required: true },
                { name: 'country', label: 'Країна' },
                { name: 'photoUrl', label: 'URL фото', type: 'url' },
                { name: 'biography', label: 'Біографія', type: 'textarea', wide: true }
            ]
        },
        genres: {
            title: 'Жанри', endpoint: '/api/genres',
            columns: [['id', 'ID'], ['name', 'Назва'], ['recordCount', 'Композицій']],
            fields: [
                { name: 'name', label: 'Назва жанру', required: true },
                { name: 'description', label: 'Опис', type: 'textarea', wide: true }
            ]
        }
    };

    const $ = (selector) => document.querySelector(selector);
    const elements = {
        tabs: [...document.querySelectorAll('.admin-tab')],
        search: $('#adminSearch'), add: $('#addItemButton'), message: $('#adminMessage'),
        loading: $('#adminLoading'), table: $('#adminTable'), head: $('#adminTableHead'),
        body: $('#adminTableBody'), empty: $('#adminEmpty'), editor: $('#adminEditor'),
        editorTitle: $('#editorTitle'), form: $('#adminForm'), fields: $('#adminFormFields'),
        close: $('#closeEditorButton'), cancel: $('#cancelEditorButton')
    };

    let currentResource = 'users';
    let currentItems = [];
    let editingId = null;

    async function request(url, options = {}) {
        const response = await fetch(url, {
            credentials: 'same-origin',
            ...options,
            headers: { 'Accept': 'application/json', ...(options.body ? { 'Content-Type': 'application/json' } : {}), ...options.headers }
        });
        if (response.status === 204) return null;
        const contentType = response.headers.get('content-type') || '';
        const payload = contentType.includes('application/json') ? await response.json() : await response.text();
        if (!response.ok) {
            const detail = payload && typeof payload === 'object'
                ? payload.message || payload.title || JSON.stringify(payload)
                : payload;
            if (response.status === 401 || response.status === 403) {
                throw new Error(response.status === 401
                    ? 'Сеанс завершився. Увійдіть до порталу знову.'
                    : 'Недостатньо прав для виконання цієї операції.');
            }
            throw new Error(detail || `Помилка HTTP ${response.status}`);
        }
        return payload;
    }

    function showMessage(text, type = 'success') {
        elements.message.textContent = text;
        elements.message.className = `admin-message ${type}`;
        elements.message.hidden = false;
        window.clearTimeout(showMessage.timeout);
        showMessage.timeout = window.setTimeout(() => { elements.message.hidden = true; }, 6000);
    }

    function cellText(item, key) {
        const value = item[key];
        if (key === 'isAdmin') return value ? 'Адміністратор' : 'Користувач';
        if (key === 'isRegistrationConfirmed') return value ? 'Підтверджено' : 'Очікує підтвердження';
        if (key === 'registrationDate' && value) return new Date(value).toLocaleDateString();
        return value === null || value === undefined || value === '' ? '—' : String(value);
    }

    function makeButton(text, className, handler, title = text) {
        const button = document.createElement('button');
        button.type = 'button'; button.className = className; button.textContent = text;
        button.title = title; button.addEventListener('click', handler);
        return button;
    }

    function renderTable() {
        const definition = config[currentResource];
        const query = elements.search.value.trim().toLocaleLowerCase();
        const visibleItems = currentItems.filter(item =>
            definition.columns.some(([key]) => cellText(item, key).toLocaleLowerCase().includes(query)));

        elements.head.replaceChildren();
        elements.body.replaceChildren();
        const headerRow = document.createElement('tr');
        definition.columns.forEach(([, label]) => {
            const th = document.createElement('th'); th.textContent = label; headerRow.append(th);
        });
        const actionsHeader = document.createElement('th'); actionsHeader.textContent = 'Дії'; headerRow.append(actionsHeader);
        elements.head.append(headerRow);

        visibleItems.forEach(item => {
            const row = document.createElement('tr');
            definition.columns.forEach(([key]) => {
                const td = document.createElement('td');
                const text = cellText(item, key);
                if (key === 'isRegistrationConfirmed') {
                    const status = document.createElement('span');
                    status.className = `admin-badge ${item.isRegistrationConfirmed ? 'approved' : 'pending'}`;
                    status.textContent = text; td.append(status);
                } else td.textContent = text;
                row.append(td);
            });
            const actions = document.createElement('td'); actions.className = 'admin-row-actions';
            if (currentResource === 'users' && !item.isRegistrationConfirmed) {
                actions.append(makeButton('Підтвердити', 'admin-small-button approve', () => setApproval(item, true)));
            }
            actions.append(makeButton('Редагувати', 'admin-small-button', () => openEditor(item)));
            actions.append(makeButton('Видалити', 'admin-small-button danger', () => removeItem(item)));
            row.append(actions); elements.body.append(row);
        });

        elements.loading.hidden = true;
        elements.table.hidden = visibleItems.length === 0;
        elements.empty.hidden = visibleItems.length !== 0;
    }

    async function loadItems() {
        elements.loading.hidden = false;
        elements.table.hidden = true;
        elements.empty.hidden = true;
        try {
            currentItems = await request(config[currentResource].endpoint) || [];
            renderTable();
        } catch (error) {
            elements.loading.hidden = true;
            showMessage(error.message, 'error');
        }
    }

    function switchResource(resource) {
        currentResource = resource;
        elements.tabs.forEach(tab => {
            const active = tab.dataset.resource === resource;
            tab.classList.toggle('active', active);
            tab.setAttribute('aria-selected', String(active));
        });
        elements.search.value = '';
        closeEditor();
        loadItems();
    }

    function createField(field, value) {
        const wrapper = document.createElement('label');
        wrapper.className = `admin-field${field.wide ? ' wide' : ''}${field.type === 'checkbox' ? ' checkbox-field' : ''}`;
        const caption = document.createElement('span'); caption.textContent = field.label; wrapper.append(caption);
        let input;
        if (field.type === 'textarea') input = document.createElement('textarea');
        else input = document.createElement('input');
        input.name = field.name;
        if (field.type === 'checkbox') {
            input.type = 'checkbox'; input.checked = Boolean(value);
            caption.prepend(input); wrapper.classList.add('checkbox-field');
        } else {
            input.type = field.type || 'text';
            input.value = value ?? '';
            input.required = Boolean(field.required);
            if (field.minLength) input.minLength = field.minLength;
            if (field.type === 'textarea') input.rows = 4;
            wrapper.append(input);
        }
        return wrapper;
    }

    function openEditor(item = null) {
        editingId = item?.id ?? null;
        const definition = config[currentResource];
        const fields = !item && definition.createFields ? definition.createFields : definition.fields;
        elements.editorTitle.textContent = item ? `Редагування: ${definition.title.toLocaleLowerCase()}` : `Новий запис: ${definition.title.toLocaleLowerCase()}`;
        elements.fields.replaceChildren(...fields.map(field => createField(field, item?.[field.name])));
        elements.editor.hidden = false;
        elements.editor.scrollIntoView({ behavior: 'smooth', block: 'start' });
        elements.fields.querySelector('input:not([type="checkbox"]), textarea')?.focus();
    }

    function closeEditor() {
        elements.editor.hidden = true;
        elements.form.reset();
        editingId = null;
    }

    function formPayload() {
        const definition = config[currentResource];
        const fields = !editingId && definition.createFields ? definition.createFields : definition.fields;
        const payload = {};
        fields.forEach(field => {
            const input = elements.form.elements.namedItem(field.name);
            payload[field.name] = field.type === 'checkbox' ? input.checked : input.value.trim();
        });
        if (editingId) payload.id = editingId;
        return payload;
    }

    async function saveItem(event) {
        event.preventDefault();
        if (!elements.form.reportValidity()) return;
        const payload = formPayload();
        const url = config[currentResource].endpoint + (editingId ? `/${editingId}` : '');
        const method = editingId ? 'PUT' : 'POST';
        try {
            await request(url, { method, body: JSON.stringify(payload) });
            showMessage(editingId ? 'Зміни успішно збережено.' : 'Запис успішно додано.');
            closeEditor(); await loadItems();
        } catch (error) { showMessage(error.message, 'error'); }
    }

    async function setApproval(item, approved) {
        const action = approved ? 'підтвердити' : 'скасувати підтвердження';
        if (!window.confirm(`Ви дійсно хочете ${action} реєстрацію користувача «${item.username}»?`)) return;
        try {
            await request(`${config.users.endpoint}/${item.id}/approval`, {
                method: 'PATCH', body: JSON.stringify({ isRegistrationConfirmed: approved })
            });
            showMessage(approved ? 'Реєстрацію користувача підтверджено.' : 'Підтвердження скасовано.');
            await loadItems();
        } catch (error) { showMessage(error.message, 'error'); }
    }

    async function removeItem(item) {
        const label = item.username || item.name;
        if (!window.confirm(`Видалити «${label}»? Цю дію неможливо скасувати.`)) return;
        try {
            await request(`${config[currentResource].endpoint}/${item.id}`, { method: 'DELETE' });
            showMessage('Запис видалено.'); await loadItems();
        } catch (error) { showMessage(error.message, 'error'); }
    }

    elements.tabs.forEach(tab => tab.addEventListener('click', () => switchResource(tab.dataset.resource)));
    elements.search.addEventListener('input', renderTable);
    elements.add.addEventListener('click', () => openEditor());
    elements.form.addEventListener('submit', saveItem);
    elements.close.addEventListener('click', closeEditor);
    elements.cancel.addEventListener('click', closeEditor);
    switchResource(currentResource);
})();

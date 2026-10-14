(() => {
    document.querySelectorAll('.order-status-select').forEach(select => {
        const actions = select.closest('.order-entry').querySelector('.order-approval-actions');
        const update = () => {
            actions.hidden = select.value === select.dataset.originalStatus;
        };
        select.addEventListener('change', update);
        select.addEventListener('click', event => event.stopPropagation());
        update();
    });
})();

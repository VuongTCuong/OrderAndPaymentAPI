(() => {
    const dialog = document.getElementById('cancel-order-dialog');
    const form = document.getElementById('cancel-order-form');
    const otherOption = document.getElementById('cancel-other-option');
    const otherReason = document.getElementById('cancel-other-reason');
    let trigger;

    document.querySelectorAll('[data-cancel-order]').forEach(button => {
        button.addEventListener('click', () => {
            trigger = button;
            form.reset();
            document.getElementById('cancel-order-id').value = button.dataset.cancelOrder;
            otherReason.disabled = true;
            otherReason.required = false;
            otherReason.setCustomValidity('');
            document.getElementById('cancel-order-number').textContent = `#${button.dataset.cancelOrder}`;
            dialog.showModal();
        });
    });

    form.addEventListener('change', () => {
        otherReason.disabled = !otherOption.checked;
        otherReason.required = otherOption.checked;
        otherReason.setCustomValidity('');
        if (otherOption.checked) otherReason.focus();
    });
    otherReason.addEventListener('input', () => otherReason.setCustomValidity(''));
    document.getElementById('cancel-order-dismiss').addEventListener('click', () => dialog.close());
    dialog.addEventListener('close', () => trigger?.focus());
})();

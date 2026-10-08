document.querySelectorAll('.quantity-control').forEach(control => {
    const input = control.querySelector('input');
    const decrease = control.querySelector('[data-step="-1"]');
    const increase = control.querySelector('[data-step="1"]');
    const syncButtons = () => {
        decrease.disabled = input.disabled || input.valueAsNumber <= Number(input.min);
        increase.disabled = input.disabled || input.valueAsNumber >= Number(input.max);
    };

    control.querySelectorAll('[data-step]').forEach(button => {
        button.addEventListener('click', () => {
            const current = Number.isFinite(input.valueAsNumber) ? input.valueAsNumber : Number(input.min);
            input.value = Math.min(Number(input.max), Math.max(Number(input.min), current + Number(button.dataset.step)));
            input.dispatchEvent(new Event('input', { bubbles: true }));
        });
    });

    input.addEventListener('input', syncButtons);
    syncButtons();
});

// Modern Admin Panel JavaScript - ES6+ with Vanilla JS

class AdminPanel {
    constructor() {
        this.charts = new Map();
        this.observers = new Map();
        this.controllers = new Map();

        // Initialize when DOM is ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.init());
        } else {
            this.init();
        }
    }

    async init() {
        console.log('Admin panel initializing...');
        try {
            await this.initializeComponents();
            this.bindEvents();
            this.loadAnimations();
            this.createToastContainer();
            console.log('Admin panel initialized successfully');
        } catch (error) {
            console.error('Error initializing admin panel:', error);
        }
    }

    async initializeComponents() {
        // Initialize Bootstrap components if available
        if (typeof bootstrap !== 'undefined') {
            this.initializeBootstrapComponents();
        }

        // Initialize DataTables if available
        if (typeof DataTable !== 'undefined') {
            await this.initializeDataTables();
        }

        // Initialize charts
        await this.initializeCharts();
    }

    initializeBootstrapComponents() {
        // Initialize tooltips
        document.querySelectorAll('[data-bs-toggle="tooltip"]')
            .forEach(element => new bootstrap.Tooltip(element));

        // Initialize popovers  
        document.querySelectorAll('[data-bs-toggle="popover"]')
            .forEach(element => new bootstrap.Popover(element));
    }

    async initializeDataTables() {
        const tables = document.querySelectorAll('.data-table');

        for (const table of tables) {
            new DataTable(table, {
                responsive: true,
                pageLength: 25,
                language: {
                    search: "Search records:",
                    lengthMenu: "Show _MENU_ entries",
                    info: "Showing _START_ to _END_ of _TOTAL_ entries",
                    infoEmpty: "No entries to show",
                    infoFiltered: "(filtered from _MAX_ total entries)"
                }
            });
        }
    }

    bindEvents() {
        // Auto-dismiss alerts
        this.handleAlerts();

        // Confirm delete actions
        this.handleDeleteActions();

        // Auto-resize textareas
        this.handleTextareas();

        // Form validation
        this.handleFormValidation();

        // Search functionality
        this.handleSearch();

        // Status updates
        this.handleStatusUpdates();

        // Loading states
        this.handleLoadingStates();

        // File uploads
        this.handleFileUploads();

        // Global error handling
        this.handleGlobalErrors();

        // Window resize handler
        this.handleWindowResize();
    }

    handleAlerts() {
        // Auto-dismiss alerts after 5 seconds
        const alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
        alerts.forEach(alert => {
            setTimeout(() => {
                alert.style.transition = 'opacity 0.5s ease-out';
                alert.style.opacity = '0';
                setTimeout(() => alert.remove(), 500);
            }, 5000);
        });

        // Manual dismiss
        document.addEventListener('click', (e) => {
            if (e.target.matches('.alert .btn-close')) {
                const alert = e.target.closest('.alert');
                alert.style.transition = 'opacity 0.3s ease-out';
                alert.style.opacity = '0';
                setTimeout(() => alert.remove(), 300);
            }
        });
    }

    handleDeleteActions() {
        document.addEventListener('click', async (e) => {
            if (e.target.matches('.btn-delete') || e.target.closest('.btn-delete')) {
                e.preventDefault();

                const button = e.target.matches('.btn-delete') ? e.target : e.target.closest('.btn-delete');
                const itemName = button.dataset.itemName || 'this item';

                const confirmed = await this.showDeleteConfirmation(itemName);

                if (confirmed) {
                    const form = button.closest('form');
                    if (form) {
                        form.submit();
                    } else {
                        window.location.href = button.getAttribute('href');
                    }
                }
            }
        });
    }

    async showDeleteConfirmation(itemName) {
        return new Promise((resolve) => {
            const modal = this.createModal({
                id: 'deleteConfirmModal',
                title: '<i class="fas fa-exclamation-triangle"></i> Confirm Deletion',
                body: `
                    <p>Are you sure you want to delete <strong>${this.escapeHtml(itemName)}</strong>?</p>
                    <p class="text-muted small">This action cannot be undone.</p>
                `,
                headerClass: 'bg-danger text-white',
                buttons: [
                    { text: 'Cancel', class: 'btn-secondary', action: () => resolve(false) },
                    { text: '<i class="fas fa-trash"></i> Delete', class: 'btn-danger', action: () => resolve(true) }
                ]
            });

            document.body.appendChild(modal);
            const modalInstance = new bootstrap.Modal(modal);
            modalInstance.show();

            modal.addEventListener('hidden.bs.modal', () => {
                modal.remove();
            });
        });
    }

    handleTextareas() {
        // Auto-resize textareas using ResizeObserver
        const textareas = document.querySelectorAll('textarea.auto-resize');

        textareas.forEach(textarea => {
            // Initial resize
            this.resizeTextarea(textarea);

            // Handle input events
            textarea.addEventListener('input', () => this.resizeTextarea(textarea));

            // Use ResizeObserver for better performance
            const observer = new ResizeObserver(() => this.resizeTextarea(textarea));
            observer.observe(textarea);
            this.observers.set(textarea, observer);
        });
    }

    resizeTextarea(textarea) {
        textarea.style.height = 'auto';
        textarea.style.height = `${textarea.scrollHeight}px`;
    }

    handleFormValidation() {
        const formControls = document.querySelectorAll('.form-control');

        formControls.forEach(control => {
            control.addEventListener('blur', () => this.validateField(control));
            control.addEventListener('input', () => this.clearValidationState(control));
        });
    }

    validateField(field) {
        const value = field.value.trim();

        // Remove existing validation classes
        field.classList.remove('is-valid', 'is-invalid');

        if (field.hasAttribute('required') && value === '') {
            field.classList.add('is-invalid');
            this.showFieldError(field, 'This field is required');
        } else if (field.type === 'email' && value !== '' && !this.validateEmail(value)) {
            field.classList.add('is-invalid');
            this.showFieldError(field, 'Please enter a valid email address');
        } else if (value !== '') {
            field.classList.add('is-valid');
            this.hideFieldError(field);
        }
    }

    clearValidationState(field) {
        field.classList.remove('is-valid', 'is-invalid');
        this.hideFieldError(field);
    }

    handleSearch() {
        const searchInputs = document.querySelectorAll('.search-input');

        searchInputs.forEach(input => {
            // Debounce search for better performance
            let timeoutId;

            input.addEventListener('input', () => {
                clearTimeout(timeoutId);
                timeoutId = setTimeout(() => {
                    const searchTerm = input.value.toLowerCase();
                    const targetSelector = input.dataset.searchTarget;
                    const searchTarget = document.querySelector(targetSelector);

                    if (searchTarget) {
                        const searchableItems = searchTarget.querySelectorAll('.searchable-item');
                        searchableItems.forEach(item => {
                            const text = item.textContent.toLowerCase();
                            item.style.display = text.includes(searchTerm) ? '' : 'none';
                        });
                    }
                }, 300);
            });
        });
    }

    handleStatusUpdates() {
        document.addEventListener('click', async (e) => {
            if (e.target.matches('.btn-status-update') || e.target.closest('.btn-status-update')) {
                e.preventDefault();

                const button = e.target.matches('.btn-status-update') ? e.target : e.target.closest('.btn-status-update');
                const originalHTML = button.innerHTML;

                try {
                    // Show loading state
                    button.innerHTML = '<div class="spinner"></div> Updating...';
                    button.disabled = true;

                    // Simulate API call (replace with actual fetch call)
                    await this.delay(1000);

                    // Show success state
                    button.innerHTML = '<i class="fas fa-check"></i> Updated';
                    button.classList.remove('btn-warning');
                    button.classList.add('btn-success');

                    // Reset after 2 seconds
                    await this.delay(2000);
                    button.innerHTML = originalHTML;
                    button.classList.remove('btn-success');
                    button.classList.add('btn-warning');
                    button.disabled = false;

                } catch (error) {
                    console.error('Status update failed:', error);
                    button.innerHTML = originalHTML;
                    button.disabled = false;
                    this.showToast('Update failed. Please try again.', 'error');
                }
            }
        });
    }

    handleLoadingStates() {
        document.addEventListener('submit', (e) => {
            if (e.target.matches('form')) {
                const submitBtn = e.target.querySelector('button[type="submit"]');
                if (submitBtn) {
                    const originalText = submitBtn.innerHTML;
                    submitBtn.innerHTML = '<div class="spinner"></div> Processing...';
                    submitBtn.disabled = true;
                    submitBtn.dataset.originalText = originalText;
                }
            }
        });
    }

    handleFileUploads() {
        const fileUploadAreas = document.querySelectorAll('.file-upload-area');

        fileUploadAreas.forEach(area => {
            // Drag and drop handlers
            area.addEventListener('dragover', (e) => {
                e.preventDefault();
                area.classList.add('drag-over');
            });

            area.addEventListener('dragleave', (e) => {
                e.preventDefault();
                area.classList.remove('drag-over');
            });

            area.addEventListener('drop', (e) => {
                e.preventDefault();
                area.classList.remove('drag-over');

                const files = Array.from(e.dataTransfer.files);
                this.handleFiles(files, area);
            });

            // File input change handler
            const fileInput = area.querySelector('.file-input');
            if (fileInput) {
                fileInput.addEventListener('change', (e) => {
                    const files = Array.from(e.target.files);
                    this.handleFiles(files, area);
                });
            }
        });
    }

    handleFiles(files, container) {
        files.forEach(file => {
            if (this.validateFile(file)) {
                this.displayFilePreview(file, container);
            } else {
                this.showToast('Invalid file type or size', 'error');
            }
        });
    }

    validateFile(file) {
        const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'application/pdf'];
        const maxSize = 5 * 1024 * 1024; // 5MB

        return allowedTypes.includes(file.type) && file.size <= maxSize;
    }

    async displayFilePreview(file, container) {
        try {
            const dataUrl = await this.readFileAsDataURL(file);

            const preview = document.createElement('div');
            preview.className = 'file-preview';
            preview.innerHTML = `
                <img src="${dataUrl}" alt="${this.escapeHtml(file.name)}" class="preview-image">
                <div class="file-info">
                    <span class="file-name">${this.escapeHtml(file.name)}</span>
                    <span class="file-size">${this.formatFileSize(file.size)}</span>
                </div>
                <button type="button" class="btn btn-sm btn-danger remove-file">
                    <i class="fas fa-times"></i>
                </button>
            `;

            // Add remove functionality
            preview.querySelector('.remove-file').addEventListener('click', () => {
                preview.remove();
            });

            const previewsContainer = container.querySelector('.file-previews');
            if (previewsContainer) {
                previewsContainer.appendChild(preview);
            }
        } catch (error) {
            console.error('Error creating file preview:', error);
            this.showToast('Error creating file preview', 'error');
        }
    }

    async initializeCharts() {
        if (typeof Chart === 'undefined') return;

        // Financial Overview Chart
        const financialCtx = document.getElementById('financialChart');
        if (financialCtx) {
            const financialChart = new Chart(financialCtx, {
                type: 'line',
                data: {
                    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                    datasets: [{
                        label: 'Income',
                        data: [8000, 9500, 12000, 11500, 13000, 12500],
                        borderColor: '#00A86B',
                        backgroundColor: 'rgba(0, 168, 107, 0.1)',
                        tension: 0.4
                    }, {
                        label: 'Expenses',
                        data: [7500, 8200, 9800, 8900, 9500, 8900],
                        borderColor: '#e74a3b',
                        backgroundColor: 'rgba(231, 74, 59, 0.1)',
                        tension: 0.4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'top',
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            ticks: {
                                callback: (value) => `$${value.toLocaleString()}`
                            }
                        }
                    }
                }
            });

            this.charts.set('financial', financialChart);
        }

        // Donations Chart
        const donationsCtx = document.getElementById('donationsChart');
        if (donationsCtx) {
            const donationsChart = new Chart(donationsCtx, {
                type: 'doughnut',
                data: {
                    labels: ['Events', 'Facilities', 'Administration', 'Programs'],
                    datasets: [{
                        data: [45000, 25000, 15000, 13000],
                        backgroundColor: [
                            '#00A86B',
                            '#4e73df',
                            '#f6c23e',
                            '#e74a3b'
                        ]
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            position: 'bottom'
                        }
                    }
                }
            });

            this.charts.set('donations', donationsChart);
        }
    }

    loadAnimations() {
        // Add fade-in animation to cards
        const cards = document.querySelectorAll('.card');
        cards.forEach(card => {
            card.classList.add('fade-in');
        });

        // Stagger animations for dashboard cards using Intersection Observer
        const statsCards = document.querySelectorAll('.stats-card');
        const observer = new IntersectionObserver((entries) => {
            entries.forEach((entry, index) => {
                if (entry.isIntersecting) {
                    setTimeout(() => {
                        entry.target.style.animationDelay = `${index * 0.1}s`;
                        entry.target.classList.add('animate-in');
                    }, index * 100);
                }
            });
        });

        statsCards.forEach(card => observer.observe(card));

        // Smooth scrolling for anchor links
        document.addEventListener('click', (e) => {
            if (e.target.matches('a[href*="#"]') && !e.target.matches('a[href="#"]')) {
                const target = e.target;
                if (location.pathname.replace(/^\//, '') === target.pathname.replace(/^\//, '') &&
                    location.hostname === target.hostname) {

                    const targetElement = document.querySelector(target.hash) ||
                        document.querySelector(`[name="${target.hash.slice(1)}"]`);

                    if (targetElement) {
                        e.preventDefault();
                        targetElement.scrollIntoView({
                            behavior: 'smooth',
                            block: 'start'
                        });

                        // Offset for fixed headers
                        setTimeout(() => {
                            window.scrollBy(0, -70);
                        }, 0);
                    }
                }
            }
        });
    }

    handleGlobalErrors() {
        // Global fetch error handler
        window.addEventListener('unhandledrejection', (e) => {
            console.error('Unhandled promise rejection:', e.reason);
            this.showToast('An unexpected error occurred. Please try again.', 'error');
        });

        // Global error handler
        window.addEventListener('error', (e) => {
            console.error('Global error:', e.error);
        });
    }

    handleWindowResize() {
        let resizeTimeout;
        window.addEventListener('resize', () => {
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(() => {
                // Resize charts
                this.charts.forEach(chart => {
                    if (chart && typeof chart.resize === 'function') {
                        chart.resize();
                    }
                });
            }, 250);
        });
    }

    createToastContainer() {
        if (!document.getElementById('toast-container')) {
            const container = document.createElement('div');
            container.id = 'toast-container';
            container.className = 'toast-container position-fixed top-0 end-0 p-3';
            document.body.appendChild(container);
        }
    }

    // Utility Methods
    createModal({ id, title, body, headerClass = '', buttons = [] }) {
        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.id = id;
        modal.setAttribute('tabindex', '-1');

        const buttonsHTML = buttons.map(btn =>
            `<button type="button" class="btn ${btn.class}" data-action="${btn.action || 'dismiss'}">${btn.text}</button>`
        ).join('');

        modal.innerHTML = `
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header ${headerClass}">
                        <h5 class="modal-title">${title}</h5>
                        <button type="button" class="btn-close ${headerClass.includes('text-white') ? 'btn-close-white' : ''}" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">${body}</div>
                    <div class="modal-footer">${buttonsHTML}</div>
                </div>
            </div>
        `;

        // Add button event listeners
        buttons.forEach((btn, index) => {
            const buttonElement = modal.querySelectorAll('.modal-footer .btn')[index];
            if (btn.action && typeof btn.action === 'function') {
                buttonElement.addEventListener('click', () => {
                    btn.action();
                    const modalInstance = bootstrap.Modal.getInstance(modal);
                    if (modalInstance) modalInstance.hide();
                });
            }
        });

        return modal;
    }

    validateEmail(email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    }

    showFieldError(field, message) {
        this.hideFieldError(field);
        const errorDiv = document.createElement('div');
        errorDiv.className = 'invalid-feedback';
        errorDiv.textContent = message;
        field.parentNode.insertBefore(errorDiv, field.nextSibling);
    }

    hideFieldError(field) {
        const existingError = field.parentNode.querySelector('.invalid-feedback');
        if (existingError) {
            existingError.remove();
        }
    }

    formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    }

    showToast(message, type = 'info') {
        const toastTypes = {
            error: 'danger',
            success: 'success',
            warning: 'warning',
            info: 'primary'
        };

        const bgClass = toastTypes[type] || 'primary';

        const toast = document.createElement('div');
        toast.className = `toast align-items-center text-white bg-${bgClass} border-0`;
        toast.setAttribute('role', 'alert');
        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">${this.escapeHtml(message)}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        `;

        const container = document.getElementById('toast-container');
        container.appendChild(toast);

        const toastInstance = new bootstrap.Toast(toast);
        toastInstance.show();

        toast.addEventListener('hidden.bs.toast', () => {
            toast.remove();
        });

        return toastInstance;
    }

    escapeHtml(unsafe) {
        return unsafe
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    readFileAsDataURL(file) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = e => resolve(e.target.result);
            reader.onerror = reject;
            reader.readAsDataURL(file);
        });
    }

    // API Methods (replace jQuery AJAX)
    async fetchJSON(url, options = {}) {
        try {
            const response = await fetch(url, {
                headers: {
                    'Content-Type': 'application/json',
                    ...options.headers
                },
                ...options
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            console.error('Fetch error:', error);
            this.showToast('Network error. Please try again.', 'error');
            throw error;
        }
    }

    async postJSON(url, data, options = {}) {
        return this.fetchJSON(url, {
            method: 'POST',
            body: JSON.stringify(data),
            ...options
        });
    }

    // Real-time notifications (modern approach)
    initializeNotifications() {
        // Use Server-Sent Events or WebSocket for real-time updates
        if (typeof EventSource !== 'undefined') {
            const eventSource = new EventSource('/api/notifications');

            eventSource.addEventListener('message', (e) => {
                const data = JSON.parse(e.data);
                this.handleNotification(data);
            });

            eventSource.addEventListener('error', (e) => {
                console.error('SSE error:', e);
                eventSource.close();

                // Fallback to polling
                setTimeout(() => this.initializePolling(), 5000);
            });
        } else {
            // Fallback to polling for older browsers
            this.initializePolling();
        }
    }

    initializePolling() {
        setInterval(async () => {
            try {
                const data = await this.fetchJSON('/api/notifications/check');
                if (data.newMessages > 0) {
                    this.updateMessageBadge(data.newMessages);
                    this.showToast(`${data.newMessages} new message(s) received`, 'info');
                }
            } catch (error) {
                console.error('Polling error:', error);
            }
        }, 60000); // Check every minute
    }

    handleNotification(data) {
        switch (data.type) {
            case 'new_message':
                this.updateMessageBadge(data.count);
                this.showToast(`${data.count} new message(s) received`, 'info');
                break;
            case 'status_update':
                this.showToast(data.message, 'success');
                break;
            default:
                console.log('Unknown notification type:', data.type);
        }
    }

    updateMessageBadge(count) {
        const badge = document.querySelector('.message-count-badge');
        if (badge) {
            badge.textContent = count;
            badge.style.display = count > 0 ? 'inline-block' : 'none';
        }
    }

    // Cleanup method
    destroy() {
        // Clean up observers
        this.observers.forEach(observer => observer.disconnect());
        this.observers.clear();

        // Clean up controllers
        this.controllers.forEach(controller => controller.abort());
        this.controllers.clear();

        // Clean up charts
        this.charts.forEach(chart => chart.destroy());
        this.charts.clear();
    }
}

// Global utility functions for backwards compatibility
window.fillCredentials = (email, password) => {
    const emailInput = document.querySelector('input[name="Email"]');
    const passwordInput = document.querySelector('input[name="Password"]');

    if (emailInput && passwordInput) {
        emailInput.value = email;
        passwordInput.value = password;

        // Trigger input events for frameworks that rely on them
        [emailInput, passwordInput].forEach(input => {
            input.dispatchEvent(new Event('input', { bubbles: true }));
            input.dispatchEvent(new Event('change', { bubbles: true }));
        });

        console.log('Credentials filled successfully');
    } else {
        console.error('Could not find email or password input fields');
    }
};

window.togglePassword = () => {
    const passwordInput = document.querySelector('input[name="Password"]');
    const toggleIcon = document.getElementById('passwordToggleIcon');

    if (passwordInput && toggleIcon) {
        const isPassword = passwordInput.type === 'password';

        passwordInput.type = isPassword ? 'text' : 'password';
        toggleIcon.classList.toggle('fa-eye', !isPassword);
        toggleIcon.classList.toggle('fa-eye-slash', isPassword);

        // Update ARIA attributes for accessibility
        passwordInput.setAttribute('aria-describedby', 'password-visibility');
        toggleIcon.setAttribute('aria-label', isPassword ? 'Hide password' : 'Show password');
    }
};

// Initialize the admin panel
const adminPanel = new AdminPanel();

// Export for module systems
if (typeof module !== 'undefined' && module.exports) {
    module.exports = AdminPanel;
}
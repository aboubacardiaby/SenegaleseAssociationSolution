// Admin JavaScript functionality - Enhanced

// Global admin object for managing state
const AdminPanel = {
    init: function() {
        this.initializeComponents();
        this.bindEvents();
        this.loadAnimations();
    },

    initializeComponents: function() {
        // Initialize tooltips
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });

        // Initialize popovers
        const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
        popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl);
        });

        // Initialize DataTables if present
        if ($.fn.DataTable && $('.data-table').length) {
            $('.data-table').DataTable({
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

        // Initialize charts if Chart.js is available
        this.initializeCharts();
    },

    bindEvents: function() {
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
    },

    handleAlerts: function() {
        // Auto-dismiss alerts after 5 seconds
        setTimeout(function() {
            $('.alert').not('.alert-permanent').fadeOut('slow');
        }, 5000);

        // Manual dismiss
        $('.alert .btn-close').click(function() {
            $(this).closest('.alert').fadeOut('fast');
        });
    },

    handleDeleteActions: function() {
        $(document).on('click', '.btn-delete', function(e) {
            e.preventDefault();
            const $this = $(this);
            const itemName = $this.data('item-name') || 'this item';
            
            // Create custom confirmation modal
            const modal = $(`
                <div class="modal fade" id="deleteConfirmModal" tabindex="-1">
                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content">
                            <div class="modal-header bg-danger text-white">
                                <h5 class="modal-title">
                                    <i class="fas fa-exclamation-triangle"></i> Confirm Deletion
                                </h5>
                                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                            </div>
                            <div class="modal-body">
                                <p>Are you sure you want to delete <strong>${itemName}</strong>?</p>
                                <p class="text-muted small">This action cannot be undone.</p>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                                <button type="button" class="btn btn-danger" id="confirmDelete">
                                    <i class="fas fa-trash"></i> Delete
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            `);
            
            $('body').append(modal);
            const modalInstance = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
            modalInstance.show();
            
            $('#confirmDelete').click(function() {
                const form = $this.closest('form');
                if (form.length) {
                    form.submit();
                } else {
                    window.location.href = $this.attr('href');
                }
                modalInstance.hide();
            });
            
            // Clean up modal after hide
            $('#deleteConfirmModal').on('hidden.bs.modal', function() {
                $(this).remove();
            });
        });
    },

    handleTextareas: function() {
        // Auto-resize textareas
        $('textarea.auto-resize').each(function() {
            this.setAttribute('style', 'height:' + (this.scrollHeight) + 'px;overflow-y:hidden;');
        }).on('input', function() {
            this.style.height = 'auto';
            this.style.height = (this.scrollHeight) + 'px';
        });
    },

    handleFormValidation: function() {
        // Real-time form validation
        $('.form-control').on('blur', function() {
            const $this = $(this);
            const value = $this.val().trim();
            
            // Remove existing validation classes
            $this.removeClass('is-valid is-invalid');
            
            if ($this.prop('required') && value === '') {
                $this.addClass('is-invalid');
                AdminPanel.showFieldError($this, 'This field is required');
            } else if ($this.attr('type') === 'email' && value !== '' && !AdminPanel.validateEmail(value)) {
                $this.addClass('is-invalid');
                AdminPanel.showFieldError($this, 'Please enter a valid email address');
            } else if (value !== '') {
                $this.addClass('is-valid');
                AdminPanel.hideFieldError($this);
            }
        });
    },

    handleSearch: function() {
        // Live search functionality
        $('.search-input').on('keyup', function() {
            const searchTerm = $(this).val().toLowerCase();
            const $searchTarget = $($(this).data('search-target'));
            
            $searchTarget.find('.searchable-item').each(function() {
                const text = $(this).text().toLowerCase();
                $(this).toggle(text.indexOf(searchTerm) > -1);
            });
        });
    },

    handleStatusUpdates: function() {
        // Handle status update buttons
        $(document).on('click', '.btn-status-update', function(e) {
            e.preventDefault();
            const $this = $(this);
            const originalText = $this.html();
            
            // Show loading state
            $this.html('<div class="spinner"></div> Updating...');
            $this.prop('disabled', true);
            
            // Simulate API call (replace with actual AJAX call)
            setTimeout(function() {
                $this.html('<i class="fas fa-check"></i> Updated');
                $this.removeClass('btn-warning').addClass('btn-success');
                
                // Reset after 2 seconds
                setTimeout(function() {
                    $this.html(originalText);
                    $this.removeClass('btn-success').addClass('btn-warning');
                    $this.prop('disabled', false);
                }, 2000);
            }, 1000);
        });
    },

    handleLoadingStates: function() {
        // Add loading states to forms
        $('form').on('submit', function() {
            const $submitBtn = $(this).find('button[type="submit"]');
            if ($submitBtn.length) {
                const originalText = $submitBtn.html();
                $submitBtn.html('<div class="spinner"></div> Processing...');
                $submitBtn.prop('disabled', true);
                
                // Store original text for potential restoration
                $submitBtn.data('original-text', originalText);
            }
        });
    },

    handleFileUploads: function() {
        // Enhanced file upload handling
        $('.file-upload-area').on('dragover', function(e) {
            e.preventDefault();
            $(this).addClass('drag-over');
        }).on('dragleave', function(e) {
            e.preventDefault();
            $(this).removeClass('drag-over');
        }).on('drop', function(e) {
            e.preventDefault();
            $(this).removeClass('drag-over');
            
            const files = e.originalEvent.dataTransfer.files;
            AdminPanel.handleFiles(files, $(this));
        });
        
        $('.file-input').on('change', function() {
            const files = this.files;
            AdminPanel.handleFiles(files, $(this).closest('.file-upload-area'));
        });
    },

    handleFiles: function(files, $container) {
        Array.from(files).forEach(file => {
            // Validate file type and size
            if (AdminPanel.validateFile(file)) {
                AdminPanel.displayFilePreview(file, $container);
            } else {
                AdminPanel.showToast('Invalid file type or size', 'error');
            }
        });
    },

    validateFile: function(file) {
        const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'application/pdf'];
        const maxSize = 5 * 1024 * 1024; // 5MB
        
        return allowedTypes.includes(file.type) && file.size <= maxSize;
    },

    displayFilePreview: function(file, $container) {
        const reader = new FileReader();
        reader.onload = function(e) {
            const preview = $(`
                <div class="file-preview">
                    <img src="${e.target.result}" alt="${file.name}" class="preview-image">
                    <div class="file-info">
                        <span class="file-name">${file.name}</span>
                        <span class="file-size">${AdminPanel.formatFileSize(file.size)}</span>
                    </div>
                    <button type="button" class="btn btn-sm btn-danger remove-file">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
            `);
            
            $container.find('.file-previews').append(preview);
        };
        reader.readAsDataURL(file);
    },

    initializeCharts: function() {
        // Initialize Chart.js charts if available
        if (typeof Chart !== 'undefined') {
            // Financial Overview Chart
            const financialCtx = document.getElementById('financialChart');
            if (financialCtx) {
                new Chart(financialCtx, {
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
                                    callback: function(value) {
                                        return '$' + value.toLocaleString();
                                    }
                                }
                            }
                        }
                    }
                });
            }
            
            // Donations Chart
            const donationsCtx = document.getElementById('donationsChart');
            if (donationsCtx) {
                new Chart(donationsCtx, {
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
            }
        }
    },

    loadAnimations: function() {
        // Add fade-in animation to cards
        $('.card').addClass('fade-in');
        
        // Stagger animations for dashboard cards
        $('.stats-card').each(function(index) {
            $(this).css('animation-delay', (index * 0.1) + 's');
        });
        
        // Smooth scrolling for anchor links
        $('a[href*="#"]').not('[href="#"]').click(function() {
            if (location.pathname.replace(/^\//, '') === this.pathname.replace(/^\//, '') && location.hostname === this.hostname) {
                let target = $(this.hash);
                target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
                if (target.length) {
                    $('html, body').animate({
                        scrollTop: target.offset().top - 70
                    }, 800);
                    return false;
                }
            }
        });
    },

    // Utility functions
    validateEmail: function(email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    },

    showFieldError: function($field, message) {
        $field.siblings('.invalid-feedback').remove();
        $field.after(`<div class="invalid-feedback">${message}</div>`);
    },

    hideFieldError: function($field) {
        $field.siblings('.invalid-feedback').remove();
    },

    formatFileSize: function(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    },

    showToast: function(message, type = 'info') {
        const toast = $(`
            <div class="toast align-items-center text-white bg-${type === 'error' ? 'danger' : type === 'success' ? 'success' : 'primary'} border-0" role="alert">
                <div class="d-flex">
                    <div class="toast-body">
                        ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `);
        
        $('#toast-container').append(toast);
        const toastInstance = new bootstrap.Toast(toast[0]);
        toastInstance.show();
        
        // Remove toast element after it's hidden
        toast.on('hidden.bs.toast', function() {
            $(this).remove();
        });
    },

    // Real-time notifications (WebSocket placeholder)
    initializeNotifications: function() {
        // Placeholder for real-time notifications
        // This would typically connect to a WebSocket or use Server-Sent Events
        setInterval(() => {
            // Simulate checking for new messages
            AdminPanel.checkNewMessages();
        }, 60000); // Check every minute
    },

    checkNewMessages: function() {
        // Placeholder for checking new messages
        // In a real application, this would make an AJAX call
        const newMessageCount = Math.floor(Math.random() * 3);
        if (newMessageCount > 0) {
            $('.message-count-badge').text(newMessageCount).show();
            AdminPanel.showToast(`${newMessageCount} new message(s) received`, 'info');
        }
    }
};

// Initialize admin panel when document is ready
$(document).ready(function() {
    AdminPanel.init();
    
    // Create toast container if it doesn't exist
    if (!$('#toast-container').length) {
        $('body').append('<div id="toast-container" class="toast-container position-fixed top-0 end-0 p-3"></div>');
    }
    
    // Initialize notifications (if needed)
    // AdminPanel.initializeNotifications();
});

// Global error handler for AJAX requests
$(document).ajaxError(function(event, xhr, settings, thrownError) {
    console.error('AJAX Error:', thrownError);
    AdminPanel.showToast('An error occurred. Please try again.', 'error');
});

// Handle window resize for responsive charts
$(window).resize(function() {
    if (typeof Chart !== 'undefined') {
        try {
            // Chart.js v3+ uses Chart.instances as an object, not array
            if (Chart.instances) {
                if (Array.isArray(Chart.instances)) {
                    // Chart.js v2.x
                    Chart.instances.forEach(function(chart) {
                        if (chart && typeof chart.resize === 'function') {
                            chart.resize();
                        }
                    });
                } else {
                    // Chart.js v3+
                    Object.values(Chart.instances).forEach(function(chart) {
                        if (chart && typeof chart.resize === 'function') {
                            chart.resize();
                        }
                    });
                }
            }
            // Alternative: Chart.js v4+ registry approach
            else if (Chart.registry && Chart.registry.getAll) {
                Chart.registry.getAll().forEach(function(chart) {
                    if (chart && typeof chart.resize === 'function') {
                        chart.resize();
                    }
                });
            }
        } catch (error) {
            console.warn('Chart resize error:', error);
        }
    }
});
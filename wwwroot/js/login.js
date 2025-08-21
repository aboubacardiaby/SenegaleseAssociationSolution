// Enhanced Login Page JavaScript
document.addEventListener('DOMContentLoaded', function() {
    console.log('Login page JavaScript loaded');
    
    // Form validation and enhancement
    const loginForm = document.getElementById('loginForm');
    const loginBtn = document.getElementById('loginBtn');
    const emailInput = document.querySelector('input[name="Email"]');
    const passwordInput = document.querySelector('input[name="Password"]');
    
    console.log('Form elements found:', {
        loginForm: !!loginForm,
        loginBtn: !!loginBtn,
        emailInput: !!emailInput,
        passwordInput: !!passwordInput
    });
    
    if (!loginForm || !loginBtn || !emailInput || !passwordInput) {
        console.error('Some form elements not found');
        return;
    }
    
    // Add click event directly to button as backup
    loginBtn.addEventListener('click', function(e) {
        console.log('Button clicked');
        // Don't prevent default here, let it submit the form
    });
    
    // Add real-time validation (optional)
    if (emailInput && passwordInput) {
        emailInput.addEventListener('blur', validateEmail);
        passwordInput.addEventListener('blur', validatePassword);
    }
    
    // Form submission handling - simplified
    if (loginForm) {
        loginForm.addEventListener('submit', function(e) {
            console.log('Form submission attempted');
            
            // Basic validation only
            const email = emailInput ? emailInput.value.trim() : '';
            const password = passwordInput ? passwordInput.value.trim() : '';
            
            if (!email || !password) {
                e.preventDefault();
                console.log('Form prevented - missing email or password');
                alert('Please enter both email and password');
                return false;
            }
            
            // Add loading state
            if (loginBtn) {
                loginBtn.classList.add('loading');
                loginBtn.disabled = true;
            }
            
            console.log('Form submitted successfully');
            // Allow form to submit
        });
    }
    
    // Add input focus effects
    document.querySelectorAll('.form-control-modern').forEach(input => {
        input.addEventListener('focus', function() {
            this.parentElement.style.transform = 'scale(1.02)';
        });
        
        input.addEventListener('blur', function() {
            this.parentElement.style.transform = 'scale(1)';
        });
    });
    
    // Enhanced checkbox functionality
    const rememberMeCheckbox = document.getElementById('rememberMe');
    const rememberMeLabel = document.querySelector('label[for="rememberMe"]');
    
    if (rememberMeCheckbox && rememberMeLabel) {
        // Add click handler to label for better UX
        rememberMeLabel.addEventListener('click', function(e) {
            if (e.target === this) {
                rememberMeCheckbox.checked = !rememberMeCheckbox.checked;
            }
        });
        
        // Add visual feedback
        rememberMeCheckbox.addEventListener('change', function() {
            if (this.checked) {
                this.parentElement.style.opacity = '1';
            } else {
                this.parentElement.style.opacity = '0.8';
            }
        });
    }
});

function validateEmail() {
    const email = document.querySelector('input[name="Email"]').value;
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    
    if (!email) {
        showFieldError('Email', 'Email is required');
        return false;
    } else if (!emailRegex.test(email)) {
        showFieldError('Email', 'Please enter a valid email address');
        return false;
    } else {
        hideFieldError('Email');
        return true;
    }
}

function validatePassword() {
    const password = document.querySelector('input[name="Password"]').value;
    
    if (!password) {
        showFieldError('Password', 'Password is required');
        return false;
    } else if (password.length < 6) {
        showFieldError('Password', 'Password must be at least 6 characters');
        return false;
    } else {
        hideFieldError('Password');
        return true;
    }
}

function validateForm() {
    const emailValid = validateEmail();
    const passwordValid = validatePassword();
    return emailValid && passwordValid;
}

function showFieldError(fieldName, message) {
    const input = document.querySelector(`input[name="${fieldName}"]`);
    const errorSpan = input.parentElement.parentElement.querySelector('.field-validation-error');
    
    input.classList.add('is-invalid');
    if (errorSpan) {
        errorSpan.textContent = message;
        errorSpan.style.display = 'block';
    }
}

function hideFieldError(fieldName) {
    const input = document.querySelector(`input[name="${fieldName}"]`);
    const errorSpan = input.parentElement.parentElement.querySelector('.field-validation-error');
    
    input.classList.remove('is-invalid');
    input.classList.add('is-valid');
    if (errorSpan) {
        errorSpan.style.display = 'none';
    }
}

function togglePassword() {
    const passwordInput = document.querySelector('input[name="Password"]');
    const toggleIcon = document.getElementById('passwordToggleIcon');
    
    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        toggleIcon.classList.remove('fa-eye');
        toggleIcon.classList.add('fa-eye-slash');
    } else {
        passwordInput.type = 'password';
        toggleIcon.classList.remove('fa-eye-slash');
        toggleIcon.classList.add('fa-eye');
    }
}

function fillCredentials(email, password) {
    document.querySelector('input[name="Email"]').value = email;
    document.querySelector('input[name="Password"]').value = password;
    
    // Add visual feedback
    const demoAccount = event.currentTarget;
    demoAccount.style.background = '#00A86B';
    demoAccount.style.color = 'white';
    
    setTimeout(() => {
        demoAccount.style.background = '';
        demoAccount.style.color = '';
    }, 1000);
    
    // Trigger validation
    validateEmail();
    validatePassword();
}

// Add particle effect on mouse move
document.addEventListener('mousemove', function(e) {
    if (Math.random() > 0.95) {
        createParticle(e.clientX, e.clientY);
    }
});

function createParticle(x, y) {
    const particle = document.createElement('div');
    particle.style.cssText = `
        position: fixed;
        left: ${x}px;
        top: ${y}px;
        width: 4px;
        height: 4px;
        background: rgba(255, 215, 0, 0.8);
        border-radius: 50%;
        pointer-events: none;
        z-index: 1000;
        animation: particleFloat 2s ease-out forwards;
    `;
    
    document.body.appendChild(particle);
    
    setTimeout(() => {
        particle.remove();
    }, 2000);
}

// Test function for debugging
function testFormSubmission() {
    console.log('Test button clicked!');
    alert('Test button works! Now checking form...');
    
    const form = document.getElementById('loginForm');
    const button = document.getElementById('loginBtn');
    
    console.log('Form element:', form);
    console.log('Button element:', button);
    
    if (form) {
        console.log('Form action:', form.action);
        console.log('Form method:', form.method);
        
        // Try to submit the form manually
        const email = document.querySelector('input[name="Email"]');
        const password = document.querySelector('input[name="Password"]');
        
        if (email && password) {
            email.value = 'admin@samn.org';
            password.value = 'Admin123!';
            console.log('Filled in demo credentials');
            
            // Submit the form
            form.submit();
        }
    }
}
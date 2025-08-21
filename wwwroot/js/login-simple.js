// Simplified Login Page JavaScript
console.log('Simplified login JavaScript loaded');

// Ensure no JavaScript interference with form submission
document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('loginForm');
    const button = document.getElementById('loginBtn');
    const checkbox = document.getElementById('rememberMe');
    const checkboxLabel = document.querySelector('label[for="rememberMe"]');
    
    console.log('Form found:', !!form);
    console.log('Button found:', !!button);
    console.log('Checkbox found:', !!checkbox);
    
    // Ensure checkbox is clickable
    if (checkbox) {
        checkbox.addEventListener('change', function() {
            console.log('Checkbox changed:', this.checked);
        });
    }
    
    // Make label clickable
    if (checkboxLabel && checkbox) {
        checkboxLabel.addEventListener('click', function(e) {
            // Prevent double-click (label already clicks checkbox by default)
            // Just add visual feedback
            console.log('Label clicked');
        });
    }
    
    // No event listeners that could interfere with submission
    // Let the form submit naturally
});

function fillCredentials(email, password) {
    const emailInput = document.querySelector('input[name="Email"]');
    const passwordInput = document.querySelector('input[name="Password"]');
    
    if (emailInput && passwordInput) {
        emailInput.value = email;
        passwordInput.value = password;
        
        // Add visual feedback
        if (typeof event !== 'undefined' && event.currentTarget) {
            const demoAccount = event.currentTarget;
            demoAccount.style.background = '#00A86B';
            demoAccount.style.color = 'white';
            
            setTimeout(() => {
                demoAccount.style.background = '';
                demoAccount.style.color = '';
            }, 1000);
        }
        
        console.log('Credentials filled successfully');
    } else {
        console.error('Could not find email or password input fields');
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
const { createApp } = Vue;

createApp({
    data() {
        return {
            user: {
                UserName: '',
                Password: ''
            },
            errors: {},
            showToast: false,
            toastMessage: '',
            toastType: 'success', // 'success' or 'error'
            showPassword: false,
            isSubmitting: false
        };
    },
    mounted() {
        // Ensure clean state on component mount
        this.resetForm();
    },
    methods: {
        togglePassword() {
            this.showPassword = !this.showPassword;
        },

        validate() {
            this.errors = {};

            if (!this.user.UserName || this.user.UserName.trim() === '') {
                this.errors.UserName = "Username is required.";
            } else if (this.user.UserName.length > 100) {
                this.errors.UserName = "Username cannot exceed 100 characters.";
            }

            if (!this.user.Password || this.user.Password.trim() === '') {
                this.errors.Password = "Password is required.";
            } else if (this.user.Password.length < 8) {
                this.errors.Password = "Password must be at least 8 characters long.";
            } else if (this.user.Password.length > 255) {
                this.errors.Password = "Password cannot exceed 255 characters.";
            }

            return Object.keys(this.errors).length === 0;
        },


        showToastMessage(message, type = 'success') {
            this.toastMessage = message;
            this.toastType = type;
            this.showToast = true;

            // Auto hide after 5 seconds
            setTimeout(() => {
                this.hideToast();
            }, 5000);
        },

        hideToast() {
            this.showToast = false;
        },

        resetForm() {
            // Force complete reset of user object
            this.user = Object.assign({}, {
                UserName: '',
                Password: ''
            });

            // Clear validation errors
            this.errors = {};

            // Reset password visibility
            this.showPassword = false;

            // Force DOM update
            this.$nextTick(() => {
                // Clear any lingering form values
                const usernameInput = document.querySelector('input[placeholder="Enter username"]');
                const passwordInput = document.querySelector('input[placeholder="Enter password"]');

                if (usernameInput) usernameInput.value = '';
                if (passwordInput) passwordInput.value = '';
            });
        },

        async submitForm() {
            if (!this.validate()) return;

            this.isSubmitting = true;

            try {
                const response = await fetch('/Users/Create', { // Fixed route
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(this.user)
                });

                const result = await response.json();

                if (response.ok) {
                    this.showToastMessage(result.message || 'User created successfully!', 'success');
                    // Reset form after successful creation
                    this.resetForm();
                    // Force Vue to update the DOM
                    this.$nextTick(() => {
                        // Additional cleanup if needed
                        console.log('Form reset completed');
                    });
                } else {
                    this.showToastMessage(result.message || 'Error occurred while creating user.', 'error');
                }

            } catch (error) {
                console.error('Error:', error);
                this.showToastMessage('Server error! Please try again.', 'error');
            } finally {
                this.isSubmitting = false;
            }
        }
    }
}).mount('#app');
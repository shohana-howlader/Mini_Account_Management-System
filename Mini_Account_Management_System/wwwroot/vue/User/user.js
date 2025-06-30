const { createApp } = Vue;

// User List App
createApp({
    data() {
        return {
            userList: [],
            isLoading: true,
            showToast: false,
            toastMessage: '',
            toastType: 'success',
            showDeleteModal: false,
            userToDelete: null,
            isDeleting: false
        };
    },
    mounted() {
        this.loadUsers();
    },
    methods: {
        async loadUsers() {
            this.isLoading = true;
            try {
                const response = await fetch('/Users/GetUsers');
                if (response.ok) {
                    const data = await response.json();
                    this.userList = data.map(user => ({
                        ...user,
                        showPassword: false
                    }));
                } else {
                    this.showToastMessage('Failed to load users.', 'error');
                }
            } catch (error) {
                console.error('Load users error:', error);
                this.showToastMessage('Server error occurred while loading users.', 'error');
            } finally {
                this.isLoading = false;
            }
        },

        togglePassword(userId) {
            const user = this.userList.find(u => u.id === userId);
            if (user) {
                user.showPassword = !user.showPassword;
            }
        },

        formatDate(dateString) {
            const date = new Date(dateString);
            return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
        },

        showToastMessage(message, type = 'success') {
            this.toastMessage = message;
            this.toastType = type;
            this.showToast = true;

            setTimeout(() => {
                this.hideToast();
            }, 5000);
        },

        hideToast() {
            this.showToast = false;
        },

        confirmDelete(user) {
            this.userToDelete = user;
            this.showDeleteModal = true;
        },

        cancelDelete() {
            this.showDeleteModal = false;
            this.userToDelete = null;
            this.isDeleting = false;
        },

        async deleteUser() {
            if (!this.userToDelete) return;

            this.isDeleting = true;

            try {
                const response = await fetch(`/Users/Delete/${this.userToDelete.id}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                    }
                });

                if (response.ok) {
                    this.userList = this.userList.filter(u => u.id !== this.userToDelete.id);
                    this.showToastMessage(`User "${this.userToDelete.userName}" deleted successfully!`, 'success');
                } else {
                    this.showToastMessage('Failed to delete user. Please try again.', 'error');
                }
            } catch (error) {
                console.error('Delete error:', error);
                this.showToastMessage('Server error occurred while deleting user.', 'error');
            } finally {
                this.cancelDelete();
            }
        }
    }
}).mount('#userListApp');


// User Create App
createApp({
    data() {
        return {
            user: {
                FullName: '',
                UserName: '',
                Password: ''
            },
            errors: {},
            showToast: false,
            toastMessage: '',
            toastType: 'success',
            showPassword: false,
            isSubmitting: false
        };
    },
    mounted() {
        this.resetForm();
    },
    methods: {
        togglePassword() {
            this.showPassword = !this.showPassword;
        },

        validate() {
            this.errors = {};

            if (!this.user.FullName || this.user.FullName.trim() === '') {
                this.errors.FullName = "FullName is required.";
            } else if (this.user.FullName.length > 100) {
                this.errors.FullName = "FullName cannot exceed 100 characters.";
            }

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

            setTimeout(() => {
                this.hideToast();
            }, 5000);
        },

        hideToast() {
            this.showToast = false;
        },

        resetForm() {
            this.user = {
                FullName: '',
                UserName: '',
                Password: ''
            };
            this.errors = {};
            this.showPassword = false;

            this.$nextTick(() => {
                const fullNameInput = document.querySelector('input[placeholder="Enter FullName"]');
                const usernameInput = document.querySelector('input[placeholder="Enter username"]');
                const passwordInput = document.querySelector('input[placeholder="Enter password"]');

                if (fullNameInput) fullNameInput.value = '';
                if (usernameInput) usernameInput.value = '';
                if (passwordInput) passwordInput.value = '';
            });
        },

        async submitForm() {
            if (!this.validate()) return;

            this.isSubmitting = true;

            try {
                const response = await fetch('/Users/Create', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(this.user)
                });

                const result = await response.json();

                if (response.ok) {
                    this.showToastMessage(result.message || 'User created successfully!', 'success');
                    this.resetForm();
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
}).mount('#userCreateApp');

// User Edit App - Enhanced Debug Version
createApp({
    data() {
        return {
            user: {
                id: 0,
                fullName: '',
                userName: '',
                password: '',
                createdDate: ''
            },
            showPassword: false,
            isLoading: true,
            isSubmitting: false,
            showToast: false,
            toastMessage: '',
            toastType: 'success',
            errors: []
        };
    },
    mounted() {
        this.loadUser();
    },
    methods: {
        async loadUser() {
            this.isLoading = true;
            console.log('Loading user...');

            try {
                // Get user ID from URL
                const pathParts = window.location.pathname.split('/');
                const userId = pathParts[pathParts.length - 1];
                console.log('User ID from URL:', userId);

                const response = await fetch(`/Users/Edit/${userId}`, {
                    method: 'GET',
                    headers: {
                        'Accept': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                console.log('Load user response status:', response.status);

                if (response.ok) {
                    const userData = await response.json();
                    console.log('User data received:', userData);

                    this.user = {
                        id: userData.id,
                        fullName: userData.fullName || '',
                        userName: userData.userName || '',
                        password: userData.password || '',
                        createdDate: userData.createdDate
                    };

                    console.log('User data set:', this.user);
                } else {
                    const errorData = await response.json().catch(() => ({}));
                    console.error('Load user error:', errorData);
                    this.showToastMessage(
                        errorData.message || 'Failed to load user data.',
                        'error'
                    );
                }
            } catch (error) {
                console.error('Load user error:', error);
                this.showToastMessage('Server error occurred while loading user.', 'error');
            } finally {
                this.isLoading = false;
            }
        },

        async updateUser() {
            // Prevent double submission
            if (this.isSubmitting) {
                console.log('Already submitting, preventing duplicate submission');
                return;
            }

            console.log('Starting user update...');
            console.log('Current user data:', this.user);

            this.isSubmitting = true;
            this.errors = [];

            try {
                // Validate required fields
                if (!this.user.fullName || !this.user.userName) {
                    this.errors.push('Full Name and Username are required');
                    this.isSubmitting = false;
                    return;
                }

                const formData = new FormData();
                formData.append('Id', this.user.id.toString());
                formData.append('FullName', this.user.fullName);
                formData.append('UserName', this.user.userName);

                // Only send password if it's not empty
                if (this.user.password && this.user.password.trim() !== '') {
                    formData.append('Password', this.user.password);
                }

                formData.append('CreatedDate', this.user.createdDate);

                // Add anti-forgery token if available
                const token = document.querySelector('input[name="__RequestVerificationToken"]');
                if (token) {
                    console.log('Anti-forgery token found:', token.value);
                    formData.append('__RequestVerificationToken', token.value);
                } else {
                    console.warn('Anti-forgery token not found!');
                }

                // Debug: Log all form data
                console.log('Form data being sent:');
                for (let [key, value] of formData.entries()) {
                    console.log(`${key}: ${value}`);
                }

                const url = `/Users/Edit/${this.user.id}`;
                console.log('Posting to URL:', url);

                const response = await fetch(url, {
                    method: 'POST',
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    body: formData
                });

                console.log('Update response status:', response.status);
                console.log('Update response headers:', Object.fromEntries(response.headers.entries()));

                if (response.ok) {
                    const contentType = response.headers.get('content-type');
                    console.log('Response content type:', contentType);

                    if (contentType && contentType.includes('application/json')) {
                        const result = await response.json();
                        console.log('JSON response:', result);

                        if (result.success) {
                            this.showToastMessage('User updated successfully!', 'success');
                            setTimeout(() => {
                                this.goToIndex();
                            }, 2000);
                        } else {
                            console.log('Update failed, validation errors:', result.errors);
                            this.handleValidationErrors(result.errors || {});
                        }
                    } else {
                        // Handle redirect or HTML response
                        const textResult = await response.text();
                        console.log('Non-JSON response received:', textResult.substring(0, 200) + '...');

                        if (response.redirected || response.url.includes('/Users')) {
                            this.showToastMessage('User updated successfully!', 'success');
                            setTimeout(() => {
                                this.goToIndex();
                            }, 2000);
                        } else {
                            this.parseValidationErrors(textResult);
                        }
                    }
                } else {
                    // Handle HTTP error responses
                    console.error('HTTP Error:', response.status, response.statusText);

                    try {
                        const errorData = await response.json();
                        console.error('Error response data:', errorData);
                        this.showToastMessage(
                            errorData.message || `Server error: ${response.status}`,
                            'error'
                        );
                    } catch (parseError) {
                        const errorText = await response.text();
                        console.error('Error response text:', errorText);
                        this.showToastMessage(
                            `Server error: ${response.status} - ${response.statusText}`,
                            'error'
                        );
                    }
                }
            } catch (error) {
                console.error('Update user error:', error);
                this.showToastMessage('Network error occurred while updating user.', 'error');
            } finally {
                this.isSubmitting = false;
                console.log('Update process completed');
            }
        },

        // Handle validation errors from server
        handleValidationErrors(errors) {
            console.log('Handling validation errors:', errors);
            this.errors = [];

            if (typeof errors === 'object' && errors !== null) {
                for (const field in errors) {
                    if (Array.isArray(errors[field])) {
                        this.errors.push(...errors[field]);
                    } else {
                        this.errors.push(errors[field]);
                    }
                }
            }

            if (this.errors.length === 0) {
                this.errors.push('Validation failed. Please check your input.');
            }

            console.log('Final error list:', this.errors);
        },

        // Parse validation errors from HTML response (fallback)
        parseValidationErrors(htmlContent) {
            console.log('Parsing HTML validation errors...');
            const parser = new DOMParser();
            const doc = parser.parseFromString(htmlContent, 'text/html');
            const errorElements = doc.querySelectorAll('.field-validation-error, .validation-summary-errors li');

            this.errors = [];
            errorElements.forEach(element => {
                if (element.textContent.trim()) {
                    this.errors.push(element.textContent.trim());
                }
            });

            if (this.errors.length === 0) {
                this.errors.push('Validation failed. Please check your input.');
            }

            console.log('Parsed errors:', this.errors);
        },

        togglePassword() {
            this.showPassword = !this.showPassword;
        },

        formatDate(dateString) {
            if (!dateString) return '';
            const date = new Date(dateString);
            return date.toLocaleDateString('en-GB') + ' ' + date.toLocaleTimeString('en-GB', {
                hour: '2-digit',
                minute: '2-digit'
            });
        },

        showToastMessage(message, type = 'success') {
            console.log(`Toast: ${type} - ${message}`);
            this.toastMessage = message;
            this.toastType = type;
            this.showToast = true;

            setTimeout(() => {
                this.hideToast();
            }, 5000);
        },

        hideToast() {
            this.showToast = false;
        },

        goToIndex() {
            window.location.href = '/Users';
        },

        viewDetails() {
            window.location.href = `/Users/Details/${this.user.id}`;
        }
    }
}).mount('#userEditApp');
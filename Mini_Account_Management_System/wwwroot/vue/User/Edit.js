    const { createApp } = Vue;

createApp({
    data() {
        return {
            user: {
                id: 0,
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
            
            try {
                // Get user ID from URL
                const pathParts = window.location.pathname.split('/');
                const userId = pathParts[pathParts.length - 1];
                
                const response = await fetch(`/Users/Edit/${userId}`, {
                    headers: {
                        'Accept': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });
                
                if (response.ok) {
                    const userData = await response.json();
                    this.user = {
                        id: userData.id,
                        userName: userData.userName,
                        password: userData.password,
                        createdDate: userData.createdDate
                    };
                } else {
                    this.showToastMessage('Failed to load user data.', 'error');
                }
            } catch (error) {
                console.error('Load user error:', error);
                this.showToastMessage('Server error occurred while loading user.', 'error');
            } finally {
                this.isLoading = false;
            }
        },
        
        async updateUser() {
            this.isSubmitting = true;
            this.errors = [];
            
            try {
                const formData = new FormData();
                formData.append('Id', this.user.id);
                formData.append('UserName', this.user.userName);
                formData.append('Password', this.user.password);
                formData.append('CreatedDate', this.user.createdDate);
                
                // Add anti-forgery token if available
                const token = document.querySelector('input[name="__RequestVerificationToken"]');
                if (token) {
                    formData.append('__RequestVerificationToken', token.value);
                }
                
                const response = await fetch(`/Users/Edit/${this.user.id}`, {
                    method: 'POST',
                    body: formData
                });
                
                if (response.ok) {
                    const result = await response.text();
                    
                    // Check if response contains success indication
                    if (result.includes('success') || response.status === 200) {
                        this.showToastMessage('User updated successfully!', 'success');
                        
                        // Redirect to index after 2 seconds
                        setTimeout(() => {
                            window.location.href = '/Users';
                        }, 2000);
                    } else {
                        // Parse validation errors if any
                        this.parseValidationErrors(result);
                    }
                } else {
                    this.showToastMessage('Failed to update user. Please try again.', 'error');
                }
            } catch (error) {
                console.error('Update user error:', error);
                this.showToastMessage('Server error occurred while updating user.', 'error');
            } finally {
                this.isSubmitting = false;
            }
        },
        
        parseValidationErrors(htmlResponse) {
            // Simple error parsing - you might need to adjust based on your server response
            const errorMatches = htmlResponse.match(/validation-summary-errors[\s\S]*?<\/ul>/);
            if (errorMatches) {
                const errorList = errorMatches[0].match(/<li.*?>(.*?)<\/li>/g);
                if (errorList) {
                    this.errors = errorList.map(item => item.replace(/<.*?>/g, '').trim());
                }
            }
            
            if (this.errors.length === 0) {
                this.errors.push('Validation error occurred. Please check your input.');
            }
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
            this.toastMessage = message;
            this.toastType = type;
            this.showToast = true;
            
            setTimeout(() => {
                this.hideToast();
            }, 5000);
        },
        
        hideToast() {
            this.showToast = false;
        }
    }
}).mount('#app');
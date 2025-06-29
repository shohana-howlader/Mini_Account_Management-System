const { createApp } = Vue;

createApp({
    data() {
        return {
            users: [],
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
                    this.users = data.map(user => ({
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
            const user = this.users.find(u => u.id === userId);
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
                    // Remove user from local array
                    this.users = this.users.filter(u => u.id !== this.userToDelete.id);
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
}).mount('#app');
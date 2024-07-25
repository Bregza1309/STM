function loadSidebar() {

    const sidebar = document.getElementById('sidebar');
    const toggleSidebar = document.getElementById('toggleSidebar');

    toggleSidebar.addEventListener('click', () => {
        sidebar.classList.toggle('active');
    });

    const userItems = document.querySelectorAll('.user-list-item');
    const selectedUser = document.getElementById('selectedUser');

    userItems.forEach((item) => {
        item.addEventListener('click', () => {
            userItems.forEach((user) => user.classList.remove('active'));
            item.classList.add('active');
            selectedUser.textContent = item.textContent;
        });
    });

}
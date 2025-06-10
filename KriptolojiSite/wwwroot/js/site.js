// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Drag and drop file handling
document.addEventListener('DOMContentLoaded', function() {
    const dropZone = document.querySelector('.border-dashed');
    const fileInput = document.querySelector('#file-upload');

    if (dropZone && fileInput) {
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            dropZone.addEventListener(eventName, preventDefaults, false);
        });

        function preventDefaults(e) {
            e.preventDefault();
            e.stopPropagation();
        }

        ['dragenter', 'dragover'].forEach(eventName => {
            dropZone.addEventListener(eventName, highlight, false);
        });

        ['dragleave', 'drop'].forEach(eventName => {
            dropZone.addEventListener(eventName, unhighlight, false);
        });

        function highlight(e) {
            dropZone.classList.add('border-purple-400');
        }

        function unhighlight(e) {
            dropZone.classList.remove('border-purple-400');
        }

        dropZone.addEventListener('drop', handleDrop, false);

        function handleDrop(e) {
            const dt = e.dataTransfer;
            const files = dt.files;
            fileInput.files = files;
            handleFiles(files);
        }

        fileInput.addEventListener('change', function() {
            handleFiles(this.files);
        });

        function handleFiles(files) {
            if (files.length > 0) {
                const file = files[0];
                if (file.size > 10 * 1024 * 1024) { // 10MB limit
                    alert('Dosya boyutu 10MB\'dan büyük olamaz!');
                    return;
                }
                // File is valid, you can proceed with the upload
                console.log('File selected:', file.name);
            }
        }
    }
});

// Copy to clipboard functionality
document.addEventListener('DOMContentLoaded', function() {
    const copyButtons = document.querySelectorAll('[title="Kopyala"]');
    
    copyButtons.forEach(button => {
        button.addEventListener('click', function() {
            const textToCopy = this.closest('.relative').querySelector('input, textarea').value;
            if (textToCopy) {
                navigator.clipboard.writeText(textToCopy).then(() => {
                    // Show success feedback
                    const originalColor = this.style.color;
                    this.style.color = '#9d4edd';
                    setTimeout(() => {
                        this.style.color = originalColor;
                    }, 1000);
                }).catch(err => {
                    console.error('Kopyalama başarısız:', err);
                });
            }
        });
    });
});

// Download functionality
document.addEventListener('DOMContentLoaded', function() {
    const downloadButtons = document.querySelectorAll('[title="İndir"]');
    
    downloadButtons.forEach(button => {
        button.addEventListener('click', function() {
            const textToDownload = this.closest('.relative').querySelector('input, textarea').value;
            if (textToDownload) {
                const blob = new Blob([textToDownload], { type: 'text/plain' });
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = 'hash.txt';
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
            }
        });
    });
});

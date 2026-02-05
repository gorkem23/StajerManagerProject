// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


showPopup = (url, title) => {
    $.ajax({
        type: "GET",
        url: url,
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (res) {
            $("#form-modal .modal-body").html(res);
            $("#form-modal .modal-title").html(title);
            $("#form-modal").modal('show');
        }
    });
}


function showSuccessMessage(message) {
    const alertHtml = `
        <div class="alert alert-success alert-dismissible fade show border-3 border-success" role="alert" style="border-left: 5px solid #198754 !important;">
            <i class="fas fa-check-circle me-2"></i><strong>Başarılı:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    $('#page-messages').html(alertHtml);
    
    setTimeout(function() {
        $('.alert').fadeOut(500, function() {
            $(this).remove();
        });
    }, 3000);
}

function showErrorMessage(message) {
    const alertHtml = `
        <div class="alert alert-danger alert-dismissible fade show border-3 border-danger" role="alert" style="border-left: 5px solid #dc3545 !important;">
            <i class="fas fa-exclamation-circle me-2"></i><strong>Hata:</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    $('#page-messages').html(alertHtml);
    
    setTimeout(function() {
        $('.alert').fadeOut(500, function() {
            $(this).remove();
        });
    }, 3000);
}

// ===== ORTAK SPA FONKSİYONLARI =====

// Ortak AJAX işlemi
async function performAjaxAction(url, method, data, successMessage, loadFunction) {
    try {
        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: data ? JSON.stringify(data) : undefined
        });
        
        // HTTP status kontrolü
        if (!response.ok) {
            if (response.status === 404) {
                throw new Error('İstenen kaynak bulunamadı.');
            } else if (response.status === 500) {
                throw new Error('Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin.');
            } else if (response.status === 401) {
                throw new Error('Yetkiniz bulunmuyor. Lütfen giriş yapın.');
            } else if (response.status === 403) {
                throw new Error('Bu işlem için yetkiniz bulunmuyor.');
            } else {
                throw new Error(`HTTP Hatası: ${response.status} - ${response.statusText}`);
            }
        }
        
        const result = await response.json();
        
        if (result.success) {
            $('#form-modal').modal('hide');
            showSuccessMessage(successMessage);
            if (loadFunction) loadFunction(); // Tabloyu yenile
        } else {
            if (result.errors) {
                showErrorMessage('Validation hataları: ' + Object.values(result.errors).flat().join(', '));
            } else {
                showErrorMessage(result.message || 'İşlem sırasında hata oluştu.');
            }
        }
        
        return result;
        
    } catch (error) {
        console.error('AJAX hatası:', error);
        
        // Network hatası kontrolü
        if (error.name === 'TypeError' && error.message.includes('fetch')) {
            showErrorMessage('Sunucuya bağlanılamıyor. İnternet bağlantınızı kontrol edin.');
        } else if (error.name === 'SyntaxError') {
            showErrorMessage('Sunucudan gelen yanıt işlenemedi.');
        } else {
            showErrorMessage('İşlem sırasında hata oluştu: ' + error.message);
        }
        
        throw error;
    }
}

// Ortak buton loading yönetimi
function setButtonLoading(buttonId, isLoading, loadingText, originalText) {
    const button = document.getElementById(buttonId);
    if (!button) return;
    
    if (isLoading) {
        button.innerHTML = `<i class="fas fa-spinner fa-spin me-1"></i>${loadingText}`;
        button.disabled = true;
    } else {
        button.innerHTML = originalText;
        button.disabled = false;
    }
}

// Ortak form validation
function validateForm(requiredFields) {
    for (const field of requiredFields) {
        const element = document.querySelector(`[name="${field}"]`);
        if (!element || !element.value.trim()) {
            showErrorMessage(`Lütfen ${field} alanını doldurun.`);
            return false;
        }
    }
    return true;
}

// ===== SPA FONKSİYONLARI =====

// Stajer Silme Fonksiyonu
window.deleteStajer = async function(stajerId) {
    if (!confirm('Bu stajer kaydını silmek istediğinizden emin misiniz?')) {
        return;
    }
    
    const deleteBtn = document.getElementById('deleteStajerBtn');
    const originalText = deleteBtn.innerHTML;
    setButtonLoading('deleteStajerBtn', true, 'Siliniyor...', originalText);
    
    try {
        await performAjaxAction(
            `/Stajers/Delete/${stajerId}`,
            'POST',
            null,
            'Stajer başarıyla silindi!',
            loadStajers
        );
    } finally {
        setButtonLoading('deleteStajerBtn', false, '', originalText);
    }
}

// Stajer Güncelleme Fonksiyonu
window.updateStajer = async function(stajerId) {
    const requiredFields = ['FullName', 'Email', 'PhoneNumber', 'DepartmanID', 'StartDate', 'EndDate'];
    if (!validateForm(requiredFields)) return;
    
    const formData = {
        StajerID: stajerId,
        FullName: document.querySelector('input[name="FullName"]').value,
        Email: document.querySelector('input[name="Email"]').value,
        PhoneNumber: document.querySelector('input[name="PhoneNumber"]').value,
        Notes: document.querySelector('textarea[name="Notes"]').value,
        UniversiteID: document.querySelector('select[name="UniversiteID"]').value || null,
        BolumID: document.querySelector('select[name="BolumID"]').value || null,
        DepartmanID: document.querySelector('select[name="DepartmanID"]').value,
        StartDate: document.querySelector('input[name="StartDate"]').value,
        EndDate: document.querySelector('input[name="EndDate"]').value
    };
    
    const updateBtn = document.getElementById('updateStajerBtn');
    const originalText = updateBtn.innerHTML;
    setButtonLoading('updateStajerBtn', true, 'Güncelleniyor...', originalText);
    
    try {
        await performAjaxAction(
            `/Stajers/Edit/${stajerId}`,
            'POST',
            formData,
            'Stajer başarıyla güncellendi!',
            loadStajers
        );
    } finally {
        setButtonLoading('updateStajerBtn', false, '', originalText);
    }
}

// Departman Kaydetme Fonksiyonu
window.saveDepartman = async function() {
    const requiredFields = ['DepartmanAdi'];
    if (!validateForm(requiredFields)) return;
    
    const formData = {
        DepartmanAdi: document.querySelector('input[name="DepartmanAdi"]').value,
        Aciklama: document.querySelector('textarea[name="Aciklama"]').value
    };
    
    const saveBtn = document.getElementById('saveDepartmanBtn');
    const originalText = saveBtn.innerHTML;
    setButtonLoading('saveDepartmanBtn', true, 'Kaydediliyor...', originalText);
    
    try {
        await performAjaxAction(
            '/Departman/Create',
            'POST',
            formData,
            'Departman başarıyla kaydedildi!',
            loadDepartmans
        );
    } finally {
        setButtonLoading('saveDepartmanBtn', false, '', originalText);
    }
}

// Departman Güncelleme Fonksiyonu
window.updateDepartman = async function(departmanId) {
    const requiredFields = ['DepartmanAdi'];
    if (!validateForm(requiredFields)) return;
    
    const formData = {
        DepartmanID: departmanId,
        DepartmanAdi: document.querySelector('input[name="DepartmanAdi"]').value,
        Aciklama: document.querySelector('textarea[name="Aciklama"]').value
    };
    
    const updateBtn = document.getElementById('updateDepartmanBtn');
    const originalText = updateBtn.innerHTML;
    setButtonLoading('updateDepartmanBtn', true, 'Güncelleniyor...', originalText);
    
    try {
        await performAjaxAction(
            `/Departman/Edit/${departmanId}`,
            'POST',
            formData,
            'Departman başarıyla güncellendi!',
            loadDepartmans
        );
    } finally {
        setButtonLoading('updateDepartmanBtn', false, '', originalText);
    }
}

// Departman Silme Fonksiyonu
window.deleteDepartman = async function(departmanId) {
    if (!confirm('Bu departmanı silmek istediğinizden emin misiniz?')) {
        return;
    }
    
    const deleteBtn = document.getElementById('deleteDepartmanBtn');
    const originalText = deleteBtn.innerHTML;
    setButtonLoading('deleteDepartmanBtn', true, 'Siliniyor...', originalText);
    
    try {
        await performAjaxAction(
            `/Departman/Delete/${departmanId}`,
            'POST',
            null,
            'Departman başarıyla silindi!',
            loadDepartmans
        );
    } finally {
        setButtonLoading('deleteDepartmanBtn', false, '', originalText);
    }
}

// Departman tablosunu yenile
async function loadDepartmans() {
    try {
        const response = await fetch('/Departman/GetDepartmans');
        const data = await response.json();
        updateDepartmanTable(data.departmans);
    } catch (error) {
        console.error('Departman tablosu yüklenirken hata:', error);
        showErrorMessage('Departman tablosu yüklenirken hata oluştu.');
    }
}

// Departman tablosunu güncelle
function updateDepartmanTable(departmans) {
    const tbody = document.getElementById('departmanTableBody');
    if (!tbody) return;
    
    tbody.innerHTML = '';
    
    departmans.forEach(departman => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${departman.departmanAdi}</td>
            <td>${departman.aciklama || ''}</td>
            <td class="text-center">
                <div class="btn-group" role="group">
                    <a onclick="showPopup('/Departman/Details/${departman.departmanID}', 'Departman Detayları')" href="javascript:void(0)" class="btn btn-info btn-sm" title="Detaylar">
                        <i class="fas fa-eye"></i>
                    </a>
                    <a onclick="showPopup('/Departman/Edit/${departman.departmanID}', 'Departman Düzenle')" href="javascript:void(0)" class="btn btn-warning btn-sm" title="Düzenle">
                        <i class="fas fa-edit"></i>
                    </a>
                    <a onclick="showPopup('/Departman/Delete/${departman.departmanID}', 'Departman Sil')" href="javascript:void(0)" class="btn btn-danger btn-sm" title="Sil">
                        <i class="fas fa-trash"></i>
                    </a>
                </div>
            </td>
        `;
        tbody.appendChild(row);
    });
}
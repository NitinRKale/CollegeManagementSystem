$(function () {

    $('#btnSubmit').on('click', function (e) {
        e.preventDefault();
        debugger;
        var $btn = $(this);
        var $form = $btn.closest('form');

        // client-side unobtrusive validation
        if (!$form.valid()) return;

        // Convert form to object, ensuring checkbox and date are handled predictably
        function formToObject($form) {
            var obj = {};
            $form.serializeArray().forEach(function (item) {
                if (obj.hasOwnProperty(item.name)) {
                    if (!Array.isArray(obj[item.name])) obj[item.name] = [obj[item.name]];
                    obj[item.name].push(item.value);
                } else {
                    obj[item.name] = item.value;
                }
            });           
            return obj;
        }

        var payloadObj = formToObject($form);
        var payload = $.param(payloadObj); // url-encode for application/x-www-form-urlencoded

        // antiforgery token from the hidden input rendered by @Html.AntiForgeryToken()
        var token = $form.find('input[name="__RequestVerificationToken"]').val();

        // UI: disable button while request is in-flight
        $btn.prop('disabled', true);

        $.ajax({
            url: '/StateMasterEFAjax/Create',
            method: 'POST',
            // do NOT set contentType => jQuery will send application/x-www-form-urlencoded; charset=UTF-8
            data: payload,
            headers: token ? { 'RequestVerificationToken': token } : {},
            success: function (resp) {
                debugger;
                if (resp && resp.success) {
                    Swal.fire(
                        {
                            title: 'Saved',
                            icon: 'success',
                            text: resp.message || 'Saved'
                        })
                        .then(function () {
                            if (resp.redirectTo) window.location.href = resp.redirectTo;
                        });
                } else {
                    Swal.fire('Error', (resp && (resp.ErrorMessage || resp.message)) || 'Failed', 'error');
                }
            },
            error: function (xhr) {
                console.error('AJAX error', xhr);
                Swal.fire('Error', 'Server error; see console/Network tab', 'error');
            },
            complete: function () {
                $btn.prop('disabled', false);
            }
        });
    });

    $('#btnUpdate').on('click', function (e) {
        e.preventDefault();
        debugger;

        var $btn = $(this);
        var $form = $btn.closest('form');

        // client-side validation
        if (!$form.valid()) return;


        // Convert form to object, ensuring checkbox and date are handled predictably
        function formToObject($form) {
            var obj = {};
            $form.serializeArray().forEach(function (item) {
                if (obj.hasOwnProperty(item.name)) {
                    if (!Array.isArray(obj[item.name])) obj[item.name] = [obj[item.name]];
                    obj[item.name].push(item.value);
                } else {
                    obj[item.name] = item.value;
                }
            });

            // Ensure numeric fields are numbers        
            if (obj.StateId) {
                obj.StateId = parseInt(obj.StateId, 10) || 0;
            }

            return obj;
        }



        var payloadObj = formToObject($form);
        var payload = $.param(payloadObj); // url-encode for application/x-www-form-urlencoded

        // Anti-forgery token
        var token = $form.find('input[name="__RequestVerificationToken"]').val();

        $btn.prop('disabled', true);

        $.ajax({
            url: '/StateMasterEFAjax/Edit',
            type: 'POST',
            data: payload,
            headers: token ? { 'RequestVerificationToken': token } : {},
            success: function (result) {
                debugger;
                if (result && result.success) {
                    Swal.fire({
                        title: 'Updated',
                        icon: 'success',
                        text: result.message || 'State updated successfully.'
                    }).then(function () {
                        if (result.redirectTo) {
                            window.location.href = result.redirectTo;
                        } else {
                            // default fallback
                            window.location.reload();
                        }
                    });
                } else {
                    var msg = (result && (result.ErrorMessage || result.message)) || 'Failed to update state.';
                    Swal.fire({ icon: 'error', title: 'Error', text: msg });
                }
            },
            error: function (xhr) {
                console.error('Update error', xhr);
                Swal.fire({ icon: 'error', title: 'Error', text: 'Server error; see console/Network tab' });
            },
            complete: function () {
                $btn.prop('disabled', false);
            }
        });
    });

    /* Delete Employee (Index listing) - delegated so it works for rows loaded by DataTables */
    $('#btnDelete').on('click', function (e) {
        e.preventDefault();
        var $btn = $(this);
        var stateId = $('#StateId').val();

        if (!stateId) {
            console.warn('No state found on delete button.');
            return;
        }

        Swal.fire({
            title: 'Are you sure?',
            text: 'This action will permanently delete the state.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Delete',
            cancelButtonText: 'Cancel'
        }).then(function (result) {
            if (!result.isConfirmed) return;
            debugger;
            // Anti-forgery token   
            var token = $('input[name="__RequestVerificationToken"]').val();

            $btn.prop('disabled', true);

            $.ajax({
                url: '/StateMasterEFAjax/Delete',
                type: 'POST',
                headers: token ? { 'RequestVerificationToken': token } : {},
                data: { stateId: stateId },
                success: function (resp) {
                    if (resp && resp.success) {
                        Swal.fire('Deleted', resp.message || 'State deleted.', 'success')
                            .then(function () {
                                if ($.fn.DataTable && $('#jqtblState').length) {
                                    $('#jqtblState').DataTable().ajax.reload(null, false);
                                } else {
                                    $btn.closest('tr').fadeOut(200, function () { $(this).remove(); });
                                }
                                if (resp.redirectTo) {
                                    window.location.href = resp.redirectTo;
                                }
                            });
                    } else {
                        Swal.fire('Error', (resp && (resp.ErrorMessage || resp.message)) || 'Failed to delete', 'error');
                    }
                },
                error: function (xhr) {
                    var err = 'An error occurred while deleting the state.';
                    try {
                        var j = xhr.responseJSON || JSON.parse(xhr.responseText);
                        if (j && (j.message || j.ErrorMessage)) err = j.message || j.ErrorMessage;
                    } catch (ex) { }
                    Swal.fire('Error', err, 'error');
                },
                complete: function () {
                    $btn.prop('disabled', false);
                }
            });
        });
    });
});
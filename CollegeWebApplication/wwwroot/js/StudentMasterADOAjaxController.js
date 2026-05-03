$(function () {

    $(function () {
        var stateSel = 'select[name="StateId"], #StateId';
        var citySel = 'select[name="CityId"], #CityId';
        var defaultUrl = '/StudentMasterADOAjax/GetCities';

        $(document).on('change', stateSel, function () {
            var $state = $(this);
            var stateId = $state.val();
            var $city = $state.closest('form').find(citySel);
            if (!$city.length) $city = $(citySel);
            $city.html('<option value="">--Select City--</option>');
            if (!stateId) return;

            var url = $state.data('cities-url') || defaultUrl;
            $city.prop('disabled', true).append('<option>Loading...</option>');

            $.getJSON(url, { stateId: stateId })
                .done(function (data) {
                    $city.empty().append('<option value="">--Select City--</option>');
                    if (!Array.isArray(data)) return;
                    data.forEach(function (item) {
                        var val = item.CityId || item.cityId || item.id;
                        var txt = item.CityName || item.cityName || item.name || '';
                        if (val != null) $city.append($('<option>').val(val).text(txt));
                    });
                })
                .fail(function () {
                    console.error('Failed to load cities for stateId:', stateId);
                })
                .always(function () {
                    $city.prop('disabled', false);
                });
        });
    });

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
                // normalize empty strings for date if needed
                if (item.name === 'BirthDate') {
                    obj[item.name] = item.value ? item.value : null;
                } else {
                    // handle repeated keys (arrays) if necessary
                    if (obj.hasOwnProperty(item.name)) {
                        if (!Array.isArray(obj[item.name])) obj[item.name] = [obj[item.name]];
                        obj[item.name].push(item.value);
                    } else {
                        obj[item.name] = item.value;
                    }
                }
            });

            // Ensure EmpStatus exists (checkbox unchecked won't be in serialized array)
            var $status = $form.find('input[name="IsActive"]');
            if ($status.length) {
                obj.EmpStatus = $status.is(':checked');
            }

            return obj;
        }

        var payloadObj = formToObject($form);
        var payload = $.param(payloadObj); // url-encode for application/x-www-form-urlencoded

        // antiforgery token from the hidden input rendered by @Html.AntiForgeryToken()
        var token = $form.find('input[name="__RequestVerificationToken"]').val();

        // UI: disable button while request is in-flight
        $btn.prop('disabled', true);

        $.ajax({
            url: '/StudentMasterADOAjax/Create',
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

        // Build JS object from form fields
        var obj = {};
        $form.serializeArray().forEach(function (item) {
            obj[item.name] = item.value;
        });

        // Ensure EmpStatus boolean is accurate
        obj.EmpStatus = $form.find('input[name="IsActive"]').is(':checked');

        // Normalize BirthDate to ISO (yyyy-MM-dd) so JSON model binder can parse it reliably.
        var bd = (obj.BirthDate || '').toString().trim();
        var isoDate = null;
        debugger;
        if (bd) {
            if (window.moment) {
                // try strict formats first, then a non-strict fallback
                var formats = [
                    'YYYY-MM-DD',
                    'YYYY/MM/DD',
                    'DD-MM-YYYY',
                    'DD/MM/YYYY',
                    'MM-DD-YYYY',
                    'MM/DD/YYYY',
                    moment.ISO_8601
                ];
                var m = moment(bd, formats, true);
                if (!m.isValid()) {
                    // non-strict fallback (more permissive)
                    m = moment(bd);
                }
                if (m.isValid()) {
                    isoDate = m.format('YYYY-MM-DD');
                } else {
                    // parsing failed: send the raw value rather than null to avoid losing the entered value
                    isoDate = bd;
                }
            } else {
                // No moment: attempt simple regex-based conversions
                var ymd = /^\s*(\d{4})[\/-](\d{1,2})[\/-](\d{1,2})\s*$/; // yyyy-mm-dd or yyyy/mm/dd
                var dmy = /^\s*(\d{1,2})[\/-](\d{1,2})[\/-](\d{4})\s*$/; // dd-mm-yyyy or dd/mm/yyyy
                var match;
                if (ymd.test(bd)) {
                    match = bd.match(ymd);
                    isoDate = match[1] + '-' + match[2].padStart(2, '0') + '-' + match[3].padStart(2, '0');
                } else if (dmy.test(bd)) {
                    match = bd.match(dmy);
                    isoDate = match[3] + '-' + match[2].padStart(2, '0') + '-' + match[1].padStart(2, '0');
                } else {
                    // last-resort: keep original string so the server can attempt parsing
                    isoDate = bd;
                }
            }
        } else {
            // empty input -> explicit null so server knows it's cleared
            isoDate = null;
        }

        obj.BirthDate = isoDate; // now either "YYYY-MM-DD", original string, or null (if empty)

        // Ensure numeric fields are numbers
        if (obj.StudentId) obj.StudentId = parseInt(obj.StudentId, 10) || 0;
        if (obj.StateId) obj.StateId = parseInt(obj.StateId, 10) || 0;
        if (obj.CityId) obj.CityId = parseInt(obj.CityId, 10) || 0;
        if (obj.CourseId) obj.CourseId = parseInt(obj.CourseId, 10) || 0;

        // Anti-forgery token
        var token = $form.find('input[name="__RequestVerificationToken"]').val();

        $btn.prop('disabled', true);

        $.ajax({
            url: '/StudentMasterADOAjax/Edit',
            type: 'POST',
            data: payload,
            headers: token ? { 'RequestVerificationToken': token } : {},
            success: function (result) {
                debugger;
                if (result && result.success) {
                    Swal.fire({
                        title: 'Updated',
                        icon: 'success',
                        text: result.message || 'Student updated successfully.'
                    }).then(function () {
                        if (result.redirectTo) {
                            window.location.href = result.redirectTo;
                        } else {
                            // default fallback
                            window.location.reload();
                        }
                    });
                } else {
                    var msg = (result && (result.ErrorMessage || result.message)) || 'Failed to update student.';
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
        var studentId = $('#StudentId').val();

        if (!studentId) {
            console.warn('No student found on delete button.');
            return;
        }

        Swal.fire({
            title: 'Are you sure?',
            text: 'This action will permanently delete the student.',
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
                url: '/StudentMasterADOAjax/Delete',
                type: 'POST',
                headers: token ? { 'RequestVerificationToken': token } : {},
                data: { studentId: studentId },
                success: function (resp) {
                    if (resp && resp.success) {
                        Swal.fire('Deleted', resp.message || 'Student deleted.', 'success')
                            .then(function () {
                                if ($.fn.DataTable && $('#jqtblStudent').length) {
                                    $('#jqtblStudent').DataTable().ajax.reload(null, false);
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
                    var err = 'An error occurred while deleting the student.';
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

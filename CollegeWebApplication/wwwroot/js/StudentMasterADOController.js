(function ($) {
    $(function () {
        var stateSel = 'select[name="StateId"], #StateId';
        var citySel = 'select[name="CityId"], #CityId';
        var defaultUrl = '/StudentMasterADO/GetCities';

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
})(jQuery);
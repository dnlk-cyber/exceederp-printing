function showApplicationVacancy(e) {
    var dropdown = e.container ? e.container.find("[name='VacancyId']").data("kendoDropDownList") : e.sender;
    if (!dropdown) return;

    var vacancy = dropdown.value() ? dropdown.dataItem() : null;
    var fields = dropdown.element.closest(".application-vacancy-fields");
    fields.find("#applicationJobTitle").val(vacancy && vacancy.JobTitle || "");
    fields.find(".application-vacancy-error").prop("hidden", !vacancy || !!vacancy.JobTitle);
}

function applicationWorkUnitParameters() {
    var vacancy = $(".application-vacancy-fields [name='VacancyId']").data("kendoDropDownList");
    return { VacancyId: vacancy ? vacancy.value() : null };
}

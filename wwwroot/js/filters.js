document.getElementById("applyFilters").addEventListener("click", function () {
    const searchText = document.getElementById("searchText").value;
    const specialty = document.getElementById("specialtyFilter").value;
    const hasLicense = document.getElementById("filterLicense").checked;
    const emailConfirmed = document.getElementById("filterEmailConfirmed").checked;

    // Build filter object
    const filters = {
        searchText: searchText,
        specialty: specialty,
        hasLicense: hasLicense,
        emailConfirmed: emailConfirmed,
    };

    // Send filters to the server via AJAX
    fetch('/Doctors/FilterDoctors', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(filters),
    })
        .then(response => response.json())
        .then(data => {
            // Update the doctor list dynamically
            const doctorList = document.getElementById("doctorList");
            doctorList.innerHTML = ""; // Clear existing list

            data.forEach(doctor => {
                const doctorCard = `
                    <div class="doctor-card">
                        <h5>${doctor.fullName} (${doctor.specialty})</h5>
                        <p>${doctor.email} | License: ${doctor.licenseNumber}</p>
                    </div>
                `;
                doctorList.innerHTML += doctorCard;
            });
        });
});

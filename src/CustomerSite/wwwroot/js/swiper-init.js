document.addEventListener('DOMContentLoaded', () => {
    const swiper = new Swiper('.category-swiper', {
        slidesPerView: 'auto', // Adjusts based on slide width
        spaceBetween: 10, // Space between slides
        freeMode: true, // Allows free scrolling
        pagination: {
            el: '.swiper-pagination',
            clickable: true,
        },
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
        breakpoints: {
            // Responsive settings
            640: {
                slidesPerView: 3,
                spaceBetween: 15,
            },
            1024: {
                slidesPerView: 5,
                spaceBetween: 20,
            },
            1280: {
                slidesPerView: 7,
                spaceBetween: 25,
            },
        },
    });
});
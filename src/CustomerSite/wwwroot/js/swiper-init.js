document.addEventListener('DOMContentLoaded', () => {
    const swiper = new Swiper('.category-swiper', {
        slidesPerView: 'auto', // Adjusts based on slide width
        spaceBetween: 10, // Space between slides
        freeMode: true, // Allows free scrolling
        breakpoints: {
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

    const swiperLatestProduct = new Swiper('.latest-product-swiper', {
        slidesPerView: 3, 
        spaceBetween: 20, 
        freeMode: true, 
        breakpoints: {
            640: {
                slidesPerView: 1,
                spaceBetween: 15,
            },
            1024: {
                slidesPerView: 2,
                spaceBetween: 20,
            },
            1280: {
                slidesPerView: 3,
                spaceBetween: 25,
            },
        },
    });
});
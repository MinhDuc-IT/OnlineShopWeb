document.addEventListener('DOMContentLoaded', () => {
    const listSliders = document.querySelector('.list-sliders');
    const btnLeft = document.querySelector('.btn-left');
    const btnRight = document.querySelector('.btn-right');
    const sliders = document.getElementsByClassName('slider-Inner');
    const indexItems = document.querySelectorAll('.index-item');
    const length = sliders.length;

    if (!listSliders || !btnLeft || !btnRight || length === 0) {
        console.error('Required elements are missing from the DOM.');
        return;
    }

    let current = 0;

    const handleChangeSlide = () => {
        let width = sliders[0].offsetWidth; 
        if (current === length - 1) {
            current = 0;
        } else {
            current++;
        }
        listSliders.style.transform = `translateX(${width * -1 * current}px)`;
        updateActiveIndex();
    };

    const updateActiveIndex = () => {
        document.querySelector('.index-item.active')?.classList.remove('active');
        indexItems[current].classList.add('active');
    };

    let handleEventChangeSlide = setInterval(handleChangeSlide, 4000);

    btnRight.addEventListener('click', () => {
        clearInterval(handleEventChangeSlide);
        handleChangeSlide();
        handleEventChangeSlide = setInterval(handleChangeSlide, 4000);
    });

    btnLeft.addEventListener('click', () => {
        clearInterval(handleEventChangeSlide);
        let width = sliders[0].offsetWidth;
        if (current === 0) {
            current = length - 1;
        } else {
            current--;
        }
        listSliders.style.transform = `translateX(${width * -1 * current}px)`;
        updateActiveIndex();
        handleEventChangeSlide = setInterval(handleChangeSlide, 4000);
    });

    listSliders.addEventListener('mouseenter', () => {
        clearInterval(handleEventChangeSlide);
    });

    listSliders.addEventListener('mouseleave', () => {
        handleEventChangeSlide = setInterval(handleChangeSlide, 4000);
    });
});

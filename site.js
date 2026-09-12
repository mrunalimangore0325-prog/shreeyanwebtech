// Shreeyan Webtech — site.js
// Vanilla JS only: sticky-header state, active nav link, full-width shelf
// panels on the nav (About/Services), the mobile drawer, and a lightweight
// scroll-reveal for [data-reveal] elements. No dependencies.

document.addEventListener('DOMContentLoaded', function () {
    var header = document.getElementById('siteHeader');

    // Sticky header shadow/border once the page has scrolled.
    if (header) {
        var setScrolled = function () {
            if (window.scrollY > 8) {
                header.classList.add('is-scrolled');
            } else {
                header.classList.remove('is-scrolled');
            }
        };
        setScrolled();
        window.addEventListener('scroll', setScrolled, { passive: true });
    }

    // Highlight the current nav link based on the active route.
    var path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.site-nav__link[href], .nav-panel__link, .mobile-drawer__list a, .mobile-drawer__sublist a').forEach(function (link) {
        var href = (link.getAttribute('href') || '').toLowerCase();
        if (href && href !== '/' && path.indexOf(href) === 0) {
            link.classList.add('active');
        } else if (href === '/' && (path === '/' || path === '/home' || path === '/home/index')) {
            link.classList.add('active');
        }
    });

    // ---- Full-width shelf panels for About / Services ----
    // Each trigger button (data-panel-trigger) opens the matching panel
    // (data-panel) anchored to the header. Click always works (so touch
    // and keyboard users are never stuck); pointer-fine devices also get
    // hover-to-open/close as a convenience. Only one panel is open at a
    // time, and clicking outside or pressing Escape closes it.
    var panelTriggers = document.querySelectorAll('[data-panel-trigger]');
    var panels = document.querySelectorAll('.nav-panel');
    var hoverCapable = window.matchMedia('(hover: hover) and (pointer: fine)').matches;

    var closeAllPanels = function () {
        panels.forEach(function (panel) {
            panel.classList.remove('is-open');
            panel.setAttribute('aria-hidden', 'true');
        });
        panelTriggers.forEach(function (trigger) {
            trigger.setAttribute('aria-expanded', 'false');
        });
    };

    var openPanel = function (name) {
        panels.forEach(function (panel) {
            var isMatch = panel.getAttribute('data-panel') === name;
            panel.classList.toggle('is-open', isMatch);
            panel.setAttribute('aria-hidden', String(!isMatch));
        });
        panelTriggers.forEach(function (trigger) {
            trigger.setAttribute('aria-expanded', String(trigger.getAttribute('data-panel-trigger') === name));
        });
    };

    if (panelTriggers.length) {
        panelTriggers.forEach(function (trigger) {
            var name = trigger.getAttribute('data-panel-trigger');

            trigger.addEventListener('click', function () {
                var alreadyOpen = trigger.getAttribute('aria-expanded') === 'true';
                if (alreadyOpen) {
                    closeAllPanels();
                } else {
                    openPanel(name);
                }
            });

            if (hoverCapable) {
                trigger.addEventListener('mouseenter', function () { openPanel(name); });
            }
        });

        if (hoverCapable) {
            // Keep a panel open while the pointer is over it, close once it
            // leaves both the trigger row and the panel itself.
            var navRow = document.querySelector('.site-nav');
            var scheduleClose = function () {
                window.setTimeout(function () {
                    var hoveringNav = navRow && navRow.matches(':hover');
                    var hoveringPanel = Array.prototype.some.call(panels, function (p) { return p.matches(':hover'); });
                    if (!hoveringNav && !hoveringPanel) {
                        closeAllPanels();
                    }
                }, 60);
            };
            if (navRow) navRow.addEventListener('mouseleave', scheduleClose);
            panels.forEach(function (panel) { panel.addEventListener('mouseleave', scheduleClose); });
        }

        document.addEventListener('click', function (e) {
            if (!e.target.closest('.site-nav') && !e.target.closest('.nav-panel')) {
                closeAllPanels();
            }
        });

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                closeAllPanels();
            }
        });
    }

    // ---- Word-by-word text reveal for [data-split-words] headings ----
    // Wraps each word in a mask span so it slides up from underneath,
    // cascading left to right. The wrapping happens now (invisible either
    // way, since the preloader is still covering the page), but the actual
    // animation is held back until the preloader has fully lifted — via
    // the 'site:revealed' event — so the reveal is something the visitor
    // actually sees, instead of playing out hidden behind the loader.
    var splitHeadings = document.querySelectorAll('[data-split-words]');
    splitHeadings.forEach(function (heading) {
        var words = heading.textContent.trim().split(/\s+/);
        heading.textContent = '';
        heading.setAttribute('aria-label', words.join(' '));

        words.forEach(function (word, i) {
            var mask = document.createElement('span');
            mask.className = 'word-mask';

            var span = document.createElement('span');
            span.className = 'word';
            span.textContent = word;
            span.style.transform = 'translateY(115%)';
            span.style.opacity = '0';
            span.style.transitionDelay = (0.05 + i * 0.06) + 's';
            span.setAttribute('aria-hidden', 'true');

            mask.appendChild(span);
            heading.appendChild(mask);
            heading.appendChild(document.createTextNode(' '));
        });
    });

    if (splitHeadings.length) {
        var playSplitHeadings = function () {
            splitHeadings.forEach(function (heading) {
                heading.classList.add('is-in');
            });
        };
        document.addEventListener('site:revealed', playSplitHeadings, { once: true });
        // Fallback in case the preloader script fails for any reason —
        // don't leave the heading invisible forever.
        window.setTimeout(playSplitHeadings, 4200);
    }

    // ---- Mobile drawer: opened by the hamburger button ----
    var navToggle = document.getElementById('navToggle');
    var mobileDrawer = document.getElementById('mobileDrawer');
    var mobileDrawerClose = document.getElementById('mobileDrawerClose');

    if (navToggle && mobileDrawer) {
        var openDrawer = function () {
            mobileDrawer.classList.add('is-open');
            mobileDrawer.setAttribute('aria-hidden', 'false');
            navToggle.classList.add('is-open');
            navToggle.setAttribute('aria-expanded', 'true');
            document.body.classList.add('nav-open');
        };

        var closeDrawer = function () {
            mobileDrawer.classList.remove('is-open');
            mobileDrawer.setAttribute('aria-hidden', 'true');
            navToggle.classList.remove('is-open');
            navToggle.setAttribute('aria-expanded', 'false');
            document.body.classList.remove('nav-open');
        };

        navToggle.addEventListener('click', function () {
            if (mobileDrawer.classList.contains('is-open')) {
                closeDrawer();
            } else {
                openDrawer();
            }
        });

        if (mobileDrawerClose) {
            mobileDrawerClose.addEventListener('click', closeDrawer);
        }

        mobileDrawer.querySelectorAll('[data-drawer-close]').forEach(function (el) {
            el.addEventListener('click', closeDrawer);
        });

        // Real navigation links close the drawer once clicked.
        mobileDrawer.querySelectorAll('.mobile-drawer__list > li > a, .mobile-drawer__sublist a, .mobile-drawer__cta').forEach(function (link) {
            link.addEventListener('click', closeDrawer);
        });

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape' && mobileDrawer.classList.contains('is-open')) {
                closeDrawer();
            }
        });

        // Accordion groups (About / Services) inside the drawer.
        mobileDrawer.querySelectorAll('[data-accordion-trigger]').forEach(function (trigger) {
            trigger.addEventListener('click', function () {
                var group = trigger.closest('.mobile-drawer__group');
                var isOpen = group.classList.contains('is-open');
                // Close any other open group so only one accordion section
                // is expanded at a time.
                mobileDrawer.querySelectorAll('.mobile-drawer__group.is-open').forEach(function (openGroup) {
                    if (openGroup !== group) {
                        openGroup.classList.remove('is-open');
                        var otherTrigger = openGroup.querySelector('[data-accordion-trigger]');
                        if (otherTrigger) otherTrigger.setAttribute('aria-expanded', 'false');
                    }
                });
                group.classList.toggle('is-open', !isOpen);
                trigger.setAttribute('aria-expanded', String(!isOpen));
            });
        });
    }

    // Hero ticker: cycles a short list of highlight phrases, Infosys-style.
    var tickerItems = document.querySelectorAll('.hero-ticker__item');
    if (tickerItems.length > 1) {
        var tickerIndex = 0;
        tickerItems[0].classList.add('is-active');
        window.setInterval(function () {
            tickerItems[tickerIndex].classList.remove('is-active');
            tickerIndex = (tickerIndex + 1) % tickerItems.length;
            tickerItems[tickerIndex].classList.add('is-active');
        }, 3200);
    } else if (tickerItems.length === 1) {
        tickerItems[0].classList.add('is-active');
    }

    // Scroll-reveal: fade/slide elements marked [data-reveal] into place.
    var revealEls = document.querySelectorAll('[data-reveal]');
    if (revealEls.length) {
        if ('IntersectionObserver' in window) {
            var observer = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('is-visible');
                        observer.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.15, rootMargin: '0px 0px -40px 0px' });

            revealEls.forEach(function (el, index) {
                el.style.transitionDelay = (Math.min(index % 4, 3) * 70) + 'ms';
                observer.observe(el);
            });
        } else {
            revealEls.forEach(function (el) {
                el.classList.add('is-visible');
            });
        }
    }

    // Animated counters: count up to [data-count-to] once the element scrolls into view.
    var counterEls = document.querySelectorAll('[data-count-to]');
    if (counterEls.length) {
        var runCounter = function (el) {
            var target = parseInt(el.getAttribute('data-count-to'), 10) || 0;
            var duration = 1400;
            var start = null;

            var step = function (timestamp) {
                if (!start) start = timestamp;
                var progress = Math.min((timestamp - start) / duration, 1);
                var eased = 1 - Math.pow(1 - progress, 3);
                el.textContent = Math.floor(eased * target);
                if (progress < 1) {
                    window.requestAnimationFrame(step);
                } else {
                    el.textContent = target;
                }
            };
            window.requestAnimationFrame(step);
        };

        if ('IntersectionObserver' in window) {
            var counterObserver = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting) {
                        runCounter(entry.target);
                        counterObserver.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.4 });

            counterEls.forEach(function (el) {
                counterObserver.observe(el);
            });
        } else {
            counterEls.forEach(function (el) {
                el.textContent = el.getAttribute('data-count-to');
            });
        }
    }

    // Flip cards (Companies page): tap to flip on touch devices, Enter/Space to flip via keyboard.
    // Desktop mouse users still get the CSS :hover flip for free.
    var flipCards = document.querySelectorAll('.flip-card');
    if (flipCards.length) {
        flipCards.forEach(function (card) {
            card.addEventListener('click', function () {
                card.classList.toggle('is-flipped');
            });
            card.addEventListener('keydown', function (e) {
                if (e.key === 'Enter' || e.key === ' ' || e.key === 'Spacebar') {
                    e.preventDefault();
                    card.classList.toggle('is-flipped');
                }
            });
        });
    }
});

// Preloader: shown while the page (images, fonts, etc.) finishes loading,
// then fades out. A minimum display time avoids an distracting flash on
// fast connections; a fallback timeout guarantees it never gets stuck.
(function () {
    var MIN_DISPLAY_MS = 500;
    var FALLBACK_MS = 4000;
    var start = Date.now();
    var revealed = false;

    function reveal() {
        if (revealed) {
            return;
        }
        revealed = true;
        var elapsed = Date.now() - start;
        var wait = Math.max(0, MIN_DISPLAY_MS - elapsed);
        window.setTimeout(function () {
            document.body.classList.add('is-loaded');
            // Let other scripts (e.g. the hero word-reveal) know the
            // loading screen has actually lifted, so they can time their
            // entrance animations to play once the visitor can see them.
            document.dispatchEvent(new Event('site:revealed'));
        }, wait);
    }

    window.addEventListener('load', reveal);
    window.setTimeout(reveal, FALLBACK_MS);
})();

// Hero background video (Infosys-style full-bleed looped video).
// Pauses the video when the tab is hidden to save CPU/battery, and falls
// back gracefully to the poster image / dark background if the browser
// can't play it (missing file, unsupported codec, data-saver mode, etc.).
(function () {
    var video = document.getElementById('heroBgVideo');
    if (!video) {
        return;
    }

    var prefersReducedMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    if (prefersReducedMotion) {
        video.removeAttribute('autoplay');
        video.pause();
        return;
    }

    // If the video source is missing/unsupported, just let the poster/
    // background gradient show instead of a broken video box.
    video.addEventListener('error', function () {
        video.style.display = 'none';
    }, true);

    document.addEventListener('visibilitychange', function () {
        if (document.hidden) {
            video.pause();
        } else {
            video.play().catch(function () {
                // Autoplay can be blocked by the browser; the poster
                // frame / overlay still looks intentional either way.
            });
        }
    });
})();
import { useEffect, useRef } from "react";

const COLORS = [
    "text-red-500",
    "text-blue-500",
    "text-green-500",
    "text-yellow-500",
    "text-purple-500",
    "text-pink-500",
    "text-orange-500",
    "text-cyan-500",
];

interface DvdBouncerProps {
    icon: React.ElementType;
}

export function DvdBouncer({ icon: BouncingIcon }: DvdBouncerProps) {
    const iconRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const icon = iconRef.current;
        if (!icon) return;

        let x = Math.random() * (window.innerWidth - 100);
        let y = Math.random() * (window.innerHeight - 100);

        let dx = 2;
        let dy = 2;

        let colorIndex = 0;
        let animationFrameId: number;

        const changeColor = () => {
            icon.classList.remove(COLORS[colorIndex]);
            colorIndex = (colorIndex + 1) % COLORS.length;
            icon.classList.add(COLORS[colorIndex]);
        };

        const animate = () => {
            const iconRect = icon.getBoundingClientRect();
            const w = window.innerWidth;
            const h = window.innerHeight;

            let bounced = false;

            if (x + iconRect.width >= w || x <= 0) {
                dx = -dx;
                bounced = true;
            }

            if (y + iconRect.height >= h || y <= 0) {
                dy = -dy;
                bounced = true;
            }

            if (bounced) {
                changeColor();
            }

            x += dx;
            y += dy;
            icon.style.transform = `translate(${x}px, ${y}px)`;

            animationFrameId = requestAnimationFrame(animate);
        };

        icon.classList.add(COLORS[0]);
        animate();

        return () => cancelAnimationFrame(animationFrameId);
    }, []);

    return (
        <div className="pointer-events-none absolute inset-0 overflow-hidden opacity-20">
            <div ref={iconRef} className="absolute transition-colors duration-300">
                <BouncingIcon className="h-24 w-24" />
            </div>
        </div>
    );
}
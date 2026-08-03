interface ArrowIconProps {
  direction?: 'left' | 'right'
}

export function ArrowIcon({ direction = 'right' }: ArrowIconProps) {
  return (
    <svg
      aria-hidden="true"
      className={`icon-arrow icon-arrow--${direction}`}
      viewBox="0 0 20 20"
    >
      <path d="M4 10h12M11 5l5 5-5 5" />
    </svg>
  )
}


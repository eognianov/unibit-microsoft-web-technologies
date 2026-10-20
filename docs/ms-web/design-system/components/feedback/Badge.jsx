import React from 'react';
export function Badge({children,tone='brand'}){
const tones={brand:{background:'var(--brand-tint-40)',color:'var(--brand-pressed)'},
success:{background:'var(--color-success-bg)',color:'var(--color-success)'},
warning:{background:'var(--color-warning-bg)',color:'var(--color-warning)'},
danger:{background:'var(--color-danger-bg)',color:'var(--color-danger)'},
neutral:{background:'var(--surface-subtle)',color:'var(--text-secondary)'}};
const t=tones[tone]||tones.brand;
return React.createElement('span',{style:{display:'inline-flex',alignItems:'center',height:20,padding:'0 8px',borderRadius:'var(--radius-pill)',
fontFamily:'var(--font-sans-text)',fontSize:'var(--fs-caption)',fontWeight:600,...t}},children);
}

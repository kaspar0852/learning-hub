const SESSION_ID_KEY = 'session-id';

function generateSessionId(): string {
  // Prefer native UUID when available.
  if (typeof crypto !== 'undefined' && 'randomUUID' in crypto) {
    return crypto.randomUUID();
  }

  // Fallback: RFC4122-ish v4 UUID.
  const bytes = new Uint8Array(16);
  for (let i = 0; i < 16; i++) bytes[i] = Math.floor(Math.random() * 256);
  bytes[6] = (bytes[6] & 0x0f) | 0x40;
  bytes[8] = (bytes[8] & 0x3f) | 0x80;
  const hex = [...bytes].map((b) => b.toString(16).padStart(2, '0')).join('');
  return `${hex.slice(0, 8)}-${hex.slice(8, 12)}-${hex.slice(12, 16)}-${hex.slice(16, 20)}-${hex.slice(20)}`;
}

export function getOrCreateSessionId(): string {
  try {
    const existing = localStorage.getItem(SESSION_ID_KEY);
    if (existing && existing.trim()) return existing;

    const created = generateSessionId();
    localStorage.setItem(SESSION_ID_KEY, created);
    return created;
  } catch {
    // If storage is blocked, fall back to a non-persisted session.
    return generateSessionId();
  }
}


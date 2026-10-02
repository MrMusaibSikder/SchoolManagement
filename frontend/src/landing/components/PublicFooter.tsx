import { Link } from "react-router-dom";
import { DEFAULT_SCHOOL_NAME } from "@/config/branding";
import { useSchoolInfo } from "../hooks/useSchoolInfo";

export function PublicFooter() {
  const { data: school } = useSchoolInfo();
  const year = new Date().getFullYear();
  const schoolName = school?.name?.trim() || DEFAULT_SCHOOL_NAME;

  return (
    <footer className="bg-ink px-6 py-12 text-paper/70 sm:px-10">
      <div className="mx-auto flex max-w-5xl flex-col gap-6 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <p className="font-display text-lg font-semibold text-paper">
            {schoolName}
          </p>
          {school?.address && <p className="mt-1 text-sm">{school.address}</p>}
        </div>

        <div className="flex flex-col gap-1 text-sm sm:items-end">
          {school?.phone && <span>{school.phone}</span>}
          {school?.email && <span>{school.email}</span>}
          <Link to="/login" className="mt-2 text-gold hover:underline">
            Staff &amp; Guardian Login →
          </Link>
        </div>
      </div>

      <div className="mx-auto mt-8 max-w-5xl border-t border-paper/10 pt-6 text-xs text-paper/40">
        © {year} {schoolName}. All rights
        reserved.
      </div>
    </footer>
  );
}

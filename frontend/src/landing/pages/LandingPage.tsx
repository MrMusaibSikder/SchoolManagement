import { HeroSection } from "../components/HeroSection";
import { StatsPlaque } from "../components/StatsPlaque";
import { NoticeBoard } from "../components/NoticeBoard";
import { PublicFooter } from "../components/PublicFooter";

export function LandingPage() {
  return (
    <main className="min-h-screen bg-paper text-ink">
      <HeroSection />
      <StatsPlaque />
      <div className="h-16 sm:h-20" />
      <NoticeBoard />
      <PublicFooter />
    </main>
  );
}
